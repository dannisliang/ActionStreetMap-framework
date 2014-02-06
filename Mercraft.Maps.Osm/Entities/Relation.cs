using System.Collections.Generic;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents a simple relation.
    /// </summary>
    public class Relation : Element
    {
        /// <summary>
        /// Creates new simple relation.
        /// </summary>
        public Relation()
        {
            this.Type = ElementType.Relation;
        }

        /// <summary>
        /// The relation members.
        /// </summary>
        public List<RelationMember> Members { get; set; }

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
                return string.Format("Relation[null]{0}", tags);
            }
            return string.Format("Relation[{0}]{1}", this.Id.Value, tags);
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        public static Relation Create(long id, params RelationMember[] members)
        {
            Relation relation = new Relation();
            relation.Id = id;
            relation.Members = new List<RelationMember>(members);
            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        public static Relation Create(long id, ICollection<Tag> tags, params RelationMember[] members)
        {
            Relation relation = new Relation();
            relation.Id = id;
            relation.Members = new List<RelationMember>(members);
            relation.Tags = tags;
            return relation;
        }
    }
}