using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Maps.Osm.Entities;

namespace ActionStreetMap.Maps.Osm.Visitors
{
    /// <summary>
    ///     Holds list of visitors.
    /// </summary>
    internal class CompositeVisitor : IElementVisitor
    {
        private readonly List<IElementVisitor> _visitors = new List<IElementVisitor>();

        public CompositeVisitor(IEnumerable<IElementVisitor> visitors)
        {
            _visitors = visitors.ToList();
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