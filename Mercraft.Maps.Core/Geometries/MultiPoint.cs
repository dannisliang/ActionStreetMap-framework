using System.Collections.Generic;
using System.Linq;

namespace Mercraft.Maps.Core.Geometries
{
    /// <summary>
    /// A multi point, a collection of zero or more points.
    /// </summary>
    public class MultiPoint : GeometryCollection
    {
        /// <summary>
        /// Creates a new multipoint string.
        /// </summary>
        public MultiPoint()
        {

        }

        /// <summary>
        /// Creates a new multipoint string.
        /// </summary>
        /// <param name="points"></param>
        public MultiPoint(IEnumerable<Point> points)
            : base(points.Cast<Geometry>())
        {

        }
    }
}
