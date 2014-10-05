using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Geometry.ThickLine
{
    public class LineElement
    {
        public float Width { get; private set; }

        public bool IsNotContinuation { get; set; }

        public List<MapPoint> Points { get; set; }

        public LineElement(List<MapPoint> points, float width)
        {
            Points = points;
            Width = width;
        }
    }
}
