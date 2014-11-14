using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.IO;
using ActionStreetMap.Osm.Entities;

namespace ActionStreetMap.Osm.Data
{
    /// <summary>
    ///     Creates pbf element source which support index list files which are result of osm splitter processing.
    /// </summary>
    public class PbfIndexListElementSource : PbfElementSource
    {
        private const string LogCategory = "OSM.pbf.index";

        private const string IndexFilePattern = "*.list";
        private const string OsmFilePattern = "{0}.osm.pbf";

        private readonly string[] _splitTo = { "to" };

        private readonly Regex _geoCoordinateRegex =
            new Regex(@"([-+]?\d{1,2}([.]\d+)?),\s*([-+]?\d{1,3}([.]\d+)?)");

        private readonly IFileSystemService _fileSystemService;
        private readonly ITrace _trace;

        private readonly List<KeyValuePair<string, BoundingBox>> _listIndex = new List<KeyValuePair<string, BoundingBox>>(32);

        private readonly List<Element> _resultElements = new List<Element>(4096);

        private string _currentFile = "";

        /// <summary>
        ///     Creates PbfIndexListElementSource.
        /// </summary>
        /// <param name="indexListPath">Index list path.</param>
        /// <param name="fileSystemService">File system service.</param>
        /// <param name="trace">Trace.</param>
        public PbfIndexListElementSource(string indexListPath, IFileSystemService fileSystemService, ITrace trace)
        {
            _fileSystemService = fileSystemService;
            _trace = trace;
            SearchAndReadIndexListFiles(indexListPath);
        }

        /// <summary>
        ///     Scans all directories recursively and processes index files
        /// </summary>
        private void SearchAndReadIndexListFiles(string folder)
        {
            _fileSystemService.GetFiles(folder, IndexFilePattern).ToList()
                .ForEach(ReadIndex);

            _fileSystemService.GetDirectories(folder, "*").ToList()
                .ForEach(SearchAndReadIndexListFiles);
        }

        /// <summary>
        ///     Reads index from list file.
        /// </summary>
        private void ReadIndex(string indexListPath)
        {
            /* Expected format:
                # List of areas
                # Generated Sun Jun 08 20:45:21 CEST 2014
                #
                00000001: 2437120,630784 to 2445312,643072
                #       : 52.294922,13.535156 to 52.470703,13.798828

                00000002: 2445312,630784 to 2455552,641024
                #       : 52.470703,13.535156 to 52.690430,13.754883
             */
            // This is just rough implementation to check idea
            // TODO improve it
            var indexFileDirectory = Path.GetDirectoryName(indexListPath);
            _trace.Output(LogCategory, String.Format("Reading index {0}..", indexFileDirectory));
            using (var reader = new StreamReader(_fileSystemService.ReadStream(indexListPath)))
            {
                // Skip three first lines
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                while (reader.Peek() >= 0)
                {
                    var fileName = Path.Combine(indexFileDirectory,
                        String.Format(OsmFilePattern, reader.ReadLine().Split(':')[0]));

                    var coordinateStrings = reader.ReadLine().Split(_splitTo, StringSplitOptions.None);
                    var minPoint = GetCoordinateFromString(coordinateStrings[0]);
                    var maxPoint = GetCoordinateFromString(coordinateStrings[1]);

                    var boundingBox = new BoundingBox(minPoint, maxPoint);

                    _listIndex.Add(new KeyValuePair<string, BoundingBox>(fileName, boundingBox));
                    _trace.Output(LogCategory, String.Format("Found [{0} ; {1}] in {2}..", minPoint, maxPoint, fileName));
                    reader.ReadLine();
                }
            }
        }

        private GeoCoordinate GetCoordinateFromString(string coordinateStr)
        {
            var coordinates = _geoCoordinateRegex.Match(coordinateStr).Value.Split(',');

            var latitude = double.Parse(coordinates[0]);
            var longitude = double.Parse(coordinates[1]);

            return new GeoCoordinate(latitude, longitude);
        }

        #region IElementSource implementation

        /// <inheritdoc />
        public override IEnumerable<Element> Get(BoundingBox bbox)
        {
            var indices = new List<int>(2);
            for (int i = 0; i < _listIndex.Count; i++)
            {
                if (bbox.Intersect(_listIndex[i].Value))
                    indices.Add(i);
            }


            foreach (var index in indices)
            {
                var filePath = _listIndex[index].Key;

                Stream fileStream = null;
                try
                {
                    // NOTE allow to cache pbf deserialization results
                    // set stream will erase cache
                    if (_currentFile != filePath)
                    {
                        _trace.Output(LogCategory, String.Format("Reading pbf {0}", filePath));
                        fileStream = _fileSystemService.ReadStream(filePath);
                        base.SetStream(fileStream);
                        _currentFile = filePath;
                    }

                    base.ResetProcessedIds();
                    base.FillElements(bbox);
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Dispose();
                }
            }
            _resultElements.Clear();
            _resultElements.AddRange(base.GetElements());
            return _resultElements;
        }

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            _resultElements.Clear();
        }

        #endregion
    }
}