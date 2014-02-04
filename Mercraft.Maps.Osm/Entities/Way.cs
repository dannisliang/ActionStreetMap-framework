using System.Collections.Generic;
using Mercraft.Maps.Core.Collections.Tags;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents a simple way.
    /// </summary>
    public class Way : Element
    {
        /// <summary>
        /// Creates a new simple way.
        /// </summary>
        public Way()
        {
            this.Type = ElementType.Way;
        }

        /// <summary>
        /// Holds the list of nodes.
        /// </summary>
        public List<long>  Nodes { get; set; }

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

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Way Create(long id, params long[] nodes)
        {
            Way way = new Way();
            way.Id = id;
            way.Nodes = new List<long>(nodes);
            return way;
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodes"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static Way Create(long id, TagsCollectionBase tags, params long[] nodes)
        {
            Way way = new Way();
            way.Id = id;
            way.Nodes = new List<long>(nodes);
            way.Tags = tags;
            return way;
        }
    }
}
