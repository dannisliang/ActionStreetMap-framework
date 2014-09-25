using System;
using System.Linq;

namespace Mercraft.Core.Elevation
{
    public class HeightMap
    {
        public int Resolution { get; set; }

        public MapPoint LeftBottomCorner { get; set; }
        public MapPoint RightUpperCorner { get; set; }
        public float AxisOffset { get; set; }

        public float MinElevation { get; set; }
        public float MaxElevation { get; set; }

        public bool IsFlat { get; set; }
        public bool IsNormalized { get; set; }

        public float[,] Data { get; set; }

        public float Size { get; set; }

        /// <summary>
        ///     Returns corresponding height for given point
        /// </summary>
        public virtual float LookupHeight(MapPoint mapPoint)
        {
            var i = (int)Math.Round((mapPoint.X - LeftBottomCorner.X) / AxisOffset);
            var j = (int)Math.Round((mapPoint.Y - LeftBottomCorner.Y) / AxisOffset);

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
