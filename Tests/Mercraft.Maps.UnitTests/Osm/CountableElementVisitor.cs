using System.Collections.Generic;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.UnitTests.Osm
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
