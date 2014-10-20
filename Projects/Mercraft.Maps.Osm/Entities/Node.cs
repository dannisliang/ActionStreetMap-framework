using Mercraft.Core;
using Mercraft.Maps.Osm.Visitors;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    ///     Represents a simple node.
    /// </summary>
    public class Node : Element
    {
        /// <summary>
        ///     The coordinates of this node.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }

        /// <summary>
        ///     True if this node located out of requested bounding box.
        ///     This flag is added to support complex tile loading logic
        /// </summary>
        public bool IsOutOfBox { get; set; }

        /// <inheritdoc />
        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitNode(this);
        }
    }
}