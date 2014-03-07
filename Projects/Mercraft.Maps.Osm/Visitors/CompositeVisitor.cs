using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    /// <summary>
    /// Holds list of visitors
    /// </summary>
    public class CompositeVisitor: IElementVisitor
    {
        private readonly List<IElementVisitor> _visitors = new List<IElementVisitor>();

        public CompositeVisitor(IEnumerable<IElementVisitor> visitors)
        {
            _visitors = visitors.ToList();
        }

        public CompositeVisitor AddVisitor(IElementVisitor visitor)
        {
            _visitors.Add(visitor);
            return this;
        }

        public CompositeVisitor RemoveVisitor(IElementVisitor visitor)
        {
            _visitors.Remove(visitor);
            return this;
        }

        #region IElementVisitor implementation

        public void VisitNode(Node node)
        {
            _visitors.ForEach(v => v.VisitNode(node));
        }

        public void VisitWay(Way way)
        {
            _visitors.ForEach(v => v.VisitWay(way));
        }

        public void VisitRelation(Relation relation)
        {
            _visitors.ForEach(v => v.VisitRelation(relation));
        }

        #endregion
    }
}
