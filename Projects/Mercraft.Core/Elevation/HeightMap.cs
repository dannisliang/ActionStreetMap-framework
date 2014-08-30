using System;

namespace Mercraft.Core.Elevation
{
    public class HeightMap
    {
        public int Resolution { get; set; }
        public float Size { get; set; }

        public BoundingBox BoundingBox { get; set; }

        public double LatitudeOffset { get; set; }
        public double LongitudeOffset { get; set; }

        public float MaxElevation { get; set; }

        public bool IsFlat { get; set; }
        public bool IsNormalized { get; set; }

        public float[,] Data { get; set; }

        /// <summary>
        ///     Returns corresponding height for given point from given heightmap
        /// </summary>
        public float LookupHeight(GeoCoordinate coordinate)
        {
            return LookupHeight(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        ///     Returns corresponding height for given point from given heightmap
        /// </summary>
        public float LookupHeight(double latitude, double longitude)
        {
            var i = (int)Math.Round((latitude - BoundingBox.MinPoint.Latitude) / LatitudeOffset);
            var j = (int)Math.Round((longitude - BoundingBox.MinPoint.Longitude) / LongitudeOffset);

            // check out of range

            var bound = Resolution - 1;
            j = j > bound ? bound : j;
            i = i > bound ? bound : i;

            j = j < 0 ? 0 : j;
            i = i < 0 ? 0 : i;

            return Data[j, i];
        }
    }
}
