using System;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    public interface IElementVisitor
    {
        void VisitNode(Node node);
        void VisitWay(Way way);
        void VisitRelation(Relation relation);
    }



    /// <summary>
    /// Helper class which provides the way to pass simply actions instead of subclassing
    /// </summary>
    public class ElementVisitor : IElementVisitor
    {
        private readonly Action<Node> _visitNode;
        private readonly Action<Relation> _visitRelation;
        private readonly Action<Way> _visitWay;

        public ElementVisitor(Action<Node> visitNode, Action<Way> visitWay, Action<Relation> visitRelation)
        {
            _visitNode = visitNode;
            _visitWay = visitWay;
            _visitRelation = visitRelation;
        }

        public void VisitNode(Node node)
        {
            _visitNode(node);
        }

        public void VisitWay(Way way)
        {
            _visitWay(way);
        }

        public void VisitRelation(Relation relation)
        {
            _visitRelation(relation);
        }
    }
}
