using Mercraft.Core;

namespace Mercraft.Models.Geometry.ThickLine
{
    public class LineElement
    {
        public float Width { get; private set; }

        public bool IsNotContinuation { get; set; }

        public MapPoint[] Points { get; set; }

        public LineElement(MapPoint[] points, float width)
        {
            Points = points;
            Width = width;
        }
    }
}
