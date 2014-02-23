
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;

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
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The coordinates of this node.
        /// </summary>
        public MapPoint Coordinate { get; set; }


        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitNode(this);
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = this.Tags.ToString();
            }
            if (!this.Id.HasValue)
            {
                return string.Format("Node[null]{0}", tags);
            }
            return string.Format("Node[{0}]{1}", this.Id.Value, tags);
        }

    }
}
