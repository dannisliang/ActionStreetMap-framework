using System.Collections.Generic;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents a simple relation.
    /// </summary>
    public class Relation : Element
    {
        /// <summary>
        /// The relation members.
        /// </summary>
        public List<RelationMember> Members { get; set; }


        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitRelation(this);
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
                return string.Format("Relation[null]{0}", tags);
            }
            return string.Format("Relation[{0}]{1}", this.Id.Value, tags);
        }
    }
}