using System.Collections.Generic;
using Mercraft.Maps.Osm.Visitors;

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
    }
}