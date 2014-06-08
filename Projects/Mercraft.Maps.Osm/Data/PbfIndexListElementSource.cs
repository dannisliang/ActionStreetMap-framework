using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Mercraft.Core;
using Mercraft.Infrastructure.Config;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Data
{
    public class PbfIndexListElementSource : PbfElementSource
    {
        private const string filePattern = "{0}.osm.pbf";

        private readonly IPathResolver _pathResolver;
        private readonly Regex _geoCoordinateRegex = 
            new Regex(@"([-+]?\d{1,2}([.]\d+)?),\s*([-+]?\d{1,3}([.]\d+)?)", RegexOptions.Compiled);

        private List<KeyValuePair<string, BoundingBox>>  _index = new List<KeyValuePair<string, BoundingBox>>();
        private Dictionary<long, Element> _elements;

        public PbfIndexListElementSource(string indexListPath, IPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
            ReadIndex(indexListPath);
        }

        /// <summary>
        /// Reads index from list file
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
            var indexListPathResolved = _pathResolver.Resolve(indexListPath);
            var indexFileDirectory = Path.GetDirectoryName(indexListPathResolved);
            using (var reader = new StreamReader(indexListPathResolved))
            {
                // Skip three first lines
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();


                while (reader.Peek() >= 0)
                {
                    var fileName = Path.Combine(indexFileDirectory, 
                        String.Format(filePattern, 
                        reader.ReadLine().Split(':')[0]));
                    
                    var coordinateStrings = reader.ReadLine().Split(new [] { "to" }, StringSplitOptions.None);
                    var minPoint = GetCoordinateFromString(coordinateStrings[0]);
                    var maxPoint = GetCoordinateFromString(coordinateStrings[1]);

                    var boundingBox = new BoundingBox(minPoint, maxPoint);

                    _index.Add(new KeyValuePair<string, BoundingBox>(fileName, boundingBox));

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
            for (int i = 0; i < _index.Count; i++)
            {
                if (bbox.Intersect(_index[i].Value))
                {
                    indecies.Add(i);
                }
            }

            var resultElements = new List<Element>();
            foreach (var index in indecies)
            {
                using (Stream fileStream = new FileStream(_index[index].Key, FileMode.Open))
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
