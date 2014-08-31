using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mercraft.Core;
using Mercraft.Infrastructure.Config;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Data
{
    public class PbfIndexListElementSource : PbfElementSource
    {
        private const string IndexFilePattern = "*.list";
        private const string OsmFilePattern = "{0}.osm.pbf";

        private readonly Regex _geoCoordinateRegex =
            new Regex(@"([-+]?\d{1,2}([.]\d+)?),\s*([-+]?\d{1,3}([.]\d+)?)");

        private readonly List<KeyValuePair<string, BoundingBox>> _listIndex =
            new List<KeyValuePair<string, BoundingBox>>();

        public PbfIndexListElementSource(string indexListPath, IPathResolver pathResolver)
        {
            SearchAndReadIndexListFiles(pathResolver.Resolve(indexListPath));
        }

        /// <summary>
        ///     Scans all directories recursively and processes index files
        /// </summary>
        private void SearchAndReadIndexListFiles(string folder)
        {
            Directory.GetFiles(folder, IndexFilePattern).ToList()
                .ForEach(ReadIndex);

            Directory.GetDirectories(folder).ToList()
                .ForEach(SearchAndReadIndexListFiles);
        }

        /// <summary>
        ///     Reads index from list file
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
            using (var reader = new StreamReader(indexListPath))
            {
                // Skip three first lines
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                while (reader.Peek() >= 0)
                {
                    var fileName = Path.Combine(indexFileDirectory,
                        String.Format(OsmFilePattern,
                            reader.ReadLine().Split(':')[0]));

                    var coordinateStrings = reader.ReadLine().Split(new[] {"to"}, StringSplitOptions.None);
                    var minPoint = GetCoordinateFromString(coordinateStrings[0]);
                    var maxPoint = GetCoordinateFromString(coordinateStrings[1]);

                    var boundingBox = new BoundingBox(minPoint, maxPoint);

                    _listIndex.Add(new KeyValuePair<string, BoundingBox>(fileName, boundingBox));

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

        public override IEnumerable<Element> Get(BoundingBox bbox)
        {
            var indecies = new List<int>();
            for (int i = 0; i < _listIndex.Count; i++)
            {
                if (bbox.Intersect(_listIndex[i].Value))
                {
                    indecies.Add(i);
                }
            }

            var resultElements = new List<Element>();
            foreach (var index in indecies)
            {
                using (Stream fileStream = new FileStream(_listIndex[index].Key, FileMode.Open))
                {
                    base.SetStream(fileStream);
                    var elements = base.Get(bbox);
                    resultElements.AddRange(elements);
                }
            }

            return resultElements;
        }

        #endregion
    }
}