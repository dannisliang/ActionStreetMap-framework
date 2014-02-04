
using Mercraft.Maps.Core.Collections.Tags;

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
                return string.Format("Node[null]{0}", tags);
            }
            return string.Format("Node[{0}]{1}", this.Id.Value, tags);
        }

        #region Construction Methods

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
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
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Node Create(long id, TagsCollectionBase tags, double latitude, double longitude)
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
