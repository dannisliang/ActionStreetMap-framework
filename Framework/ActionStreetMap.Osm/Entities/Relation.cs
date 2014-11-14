using System.Collections.Generic;
using ActionStreetMap.Osm.Visitors;

namespace ActionStreetMap.Osm.Entities
{
    /// <summary>
    ///     Represents a simple relation.
    /// </summary>
    public class Relation : Element
    {
        /// <summary>
        ///     The relation members.
        /// </summary>
        public List<RelationMember> Members { get; set; }

        /// <inheritdoc />
        public override void Accept(IElementVisitor elementVisitor)
        {
            elementVisitor.VisitRelation(this);
        }
    }
}