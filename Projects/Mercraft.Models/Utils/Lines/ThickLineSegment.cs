
using Mercraft.Models.Utils.Geometry;

namespace Mercraft.Models.Utils.Lines
{
    public class ThickLineSegment
    {
        public Segment Left;
        public Segment Right;

        public ThickLineSegment(Segment left, Segment right)
        {
            Left = left;
            Right = right;
        }
    }
}
