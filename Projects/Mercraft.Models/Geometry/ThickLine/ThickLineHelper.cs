namespace Mercraft.Models.Geometry.ThickLine
{
    internal static class ThickLineHelper
    {
        internal static Direction GetDirection(ThickLineSegment first, ThickLineSegment second)
        {
            // just straight line with shared point
            var area = first.Left.Start.x * (first.Left.End.z - second.Left.End.z) +
                       first.Left.End.x * (second.Left.End.z - first.Left.Start.z) +
                       second.Left.End.x * (first.Left.Start.z - first.Left.End.z);
            if (area < 0.1)
                return Direction.Straight;

            if (SegmentUtils.Intersect(first.Left, second.Left))
                return Direction.Left;

            if (SegmentUtils.Intersect(first.Right, second.Right))
                return Direction.Right;

            return Direction.Straight;
        }

        internal enum Direction
        {
            Straight,
            Left,
            Right
        }
    }
}
