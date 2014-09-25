
namespace Mercraft.Models.Geometry.ThickLine
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
