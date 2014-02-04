namespace Mercraft.Math.Primitives.Enumerators.Points
{
    /// <summary>
    /// Interface representing an enumerable with a list of points.
    /// </summary>
    internal interface IPointList
    {
        /// <summary>
        /// Returns the count of points.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the point at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        PointF2D this[int idx]
        {
            get;
        }
    }
}
