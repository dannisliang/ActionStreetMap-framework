using System.Collections.Generic;
using Mercraft.Maps.Core;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents a simple node.
    /// </summary>
    public class Node : Element
    {
        /// <summary>
        /// Creates a new simple node.
        /// </summary>
        public Node()
        {
            this.Type = ElementType.Node;
        }

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
        public GeoCoordinate Coordinate { get; set; }

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

        #region Construction Methods

        /// <summary>
        /// Creates a new node.
        /// </summary>
        public static Node Create(long id, double latitude, double longitude)
        {
            Node node = new Node();
            node.Id = id;
            node.Latitude = latitude;
            node.Longitude = longitude;
            return node;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        public static Node Create(long id, ICollection<Tag> tags, double latitude, double longitude)
        {
            Node node = new Node();
            node.Id = id;
            node.Latitude = latitude;
            node.Longitude = longitude;
            node.Tags = tags;
            return node;
        }

        #endregion
    }
}
