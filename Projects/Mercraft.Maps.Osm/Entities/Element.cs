using System.Collections.Generic;
using ActionStreetMap.Maps.Osm.Visitors;

namespace ActionStreetMap.Maps.Osm.Entities
{
    /// <summary>
    ///     Primive used as a base class for any osm object that has a meaning on the map (NodeIds, Ways and Relations).
    /// </summary>
    public abstract class Element
    {
        /// <summary>
        ///     The id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     The tags.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        ///     Accepts visitor.
        /// </summary>
        /// <param name="elementVisitor">Element visitor.</param>
        public abstract void Accept(IElementVisitor elementVisitor);

        /// <inheritdoc />
        public override string ToString()
        {
            string tags = "{no tags}";
            if (Tags != null && Tags.Count > 0)
            {
                tags = "tags:{";
                foreach (var tag in Tags)
                {
                    tags += string.Format("{0}:{1},", tag.Key, tag.Value);
                }
                tags += "}";
            }
            return string.Format("{0}[{1}]{2}", GetType().Name, Id, tags);
        }
    }
}