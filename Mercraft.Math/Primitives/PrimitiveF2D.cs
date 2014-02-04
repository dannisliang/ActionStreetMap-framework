namespace Mercraft.Math.Primitives
{
    /// <summary>
    /// An abstract class serving as the base-type for all primitives.
    /// </summary>
    public abstract class PrimitiveF2D
    {
        /// <summary>
        /// Calculates the distance of this primitive to the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract double Distance(PointF2D p);
    }
}
