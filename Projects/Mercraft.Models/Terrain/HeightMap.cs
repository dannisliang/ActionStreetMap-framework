using System;
using Mercraft.Core;

namespace Mercraft.Models.Terrain
{
    public class HeightMap
    {
        public int Resolution { get; set; }
        public float Size { get; set; }
        public float MaxElevation { get; set; }

        public bool IsFlat { get; set; }

        public float[,] Data { get; set; }

        /// <summary>
        ///     Returns corresponding height for given point from given heightmap
        /// </summary>
        public float LookupHeight(GeoCoordinate coordinate)
        {
            throw new NotImplementedException();
        }
    }
}
