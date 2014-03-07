
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents a simple node.
    /// </summary>
    public class Node : Element
    {
        /// <summary>
        /// The latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The coordinates of this node.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }


        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitNode(this);
        }
    }
}
