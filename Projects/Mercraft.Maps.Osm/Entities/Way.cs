using System.Collections.Generic;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents a simple way.
    /// </summary>
    public class Way : Element
    {
        /// <summary>
        /// Holds the list of nodes.
        /// </summary>
        public List<long>  NodeIds { get; set; }

        public List<Node> Nodes { get; set; }


        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitWay(this);
        }


        /// <summary>
        /// Returns all the ponts in this way in the same order as the nodes.
        /// </summary>
        /// <returns></returns>
        public List<MapPoint> GetPoints()
        {
            var coordinates = new List<MapPoint>();

            for (int idx = 0; idx < this.Nodes.Count; idx++)
            {
                coordinates.Add(this.Nodes[idx].Coordinate);
            }

            return coordinates;
        }

        /// <summary>
        /// Returns true if this way is closed (firstnode == lastnode).
        /// </summary>
        /// <returns></returns>
        public bool IsClosed()
        {
            return this.Nodes != null &&
                this.Nodes.Count > 1 &&
                this.Nodes[0].Id == this.Nodes[this.Nodes.Count - 1].Id;
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = this.Tags.ToString();
            }
            if (!this.Id.HasValue)
            {
                return string.Format("Way[null]{0}", tags);
            }
            return string.Format("Way[{0}]{1}", this.Id.Value, tags);
        }
    }
}
