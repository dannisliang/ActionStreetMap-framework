using System.Collections.Generic;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Visitors;

namespace Mercraft.Maps.UnitTests.Stubs
{
    public class CountableElementVisitor: IElementVisitor
    {
        private List<Element> _elements = new List<Element>();

        public void VisitNode(Node node)
        {
            _elements.Add(node);
        }

        public void VisitRelation(Relation relation)
        {
            _elements.Add(relation);
        }

        public void VisitWay(Way way)
        {
            _elements.Add(way);
        }

        public IList<Element> Elements
        {
            get
            {
                return _elements;
            }
        }
    }
}
