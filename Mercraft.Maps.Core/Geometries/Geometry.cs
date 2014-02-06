namespace Mercraft.Maps.Core.Geometries
{
    /// <summary>
    /// Base class for all geometries.
    /// </summary>
    public abstract class Geometry
    {
        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public abstract GeoCoordinateBox Box
        {
            get;
        }

        /// <summary>
        /// Returns true if this geometry is inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public abstract bool IsInside(GeoCoordinateBox box);

    }
}
