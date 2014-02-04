
using System.Collections.Generic;
using System.Linq;

namespace Mercraft.Maps.Core.Geometries
{
    /// <summary>
    /// A multi line string, a collection of one or more linestring instances.
    /// </summary>
    public class MultiLineString : GeometryCollection
    {
        /// <summary>
        /// Creates a new multiline string.
        /// </summary>
        public MultiLineString()
        {

        }

        /// <summary>
        /// Creates a new multiline string.
        /// </summary>
        /// <param name="lineStrings"></param>
        public MultiLineString(IEnumerable<LineString> lineStrings)
            : base(lineStrings.Cast<Geometry>())
        {

        }
    }
}
