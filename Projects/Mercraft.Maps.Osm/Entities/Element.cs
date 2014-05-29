using System;
using System.Collections.Generic;
using Mercraft.Maps.Osm.Visitors;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Primive used as a base class for any osm object that has a meaning on the map (NodeIds, Ways and Relations).
    /// </summary>
    public abstract class Element
    {
        protected Element()
        {
        }

        /// <summary>
        /// The id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The tags.
        /// </summary>
        public ICollection<KeyValuePair<string, string>> Tags { get; set; }

        public abstract void Accept(IElementVisitor elementVisitor);

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = "tags:{";
                foreach (var tag in Tags)
                {
                    tags += string.Format("{0}:{1},", tag.Key, tag.Value);
                }
                tags += "}";
            }
            return string.Format("{0}[{1}]{2}", GetType().Name, this.Id, tags);
        }
    }
}
