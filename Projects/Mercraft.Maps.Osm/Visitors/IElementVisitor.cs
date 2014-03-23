using System;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    /// <summary>
    /// Visitor for OSM elements
    /// </summary>
    public interface IElementVisitor
    {
        void VisitNode(Node node);
        void VisitWay(Way way);
        void VisitRelation(Relation relation);
    }

    /// <summary>
    /// Helper class which is used for implementing separate element visitors
    /// </summary>
    public class ElementVisitor : IElementVisitor
    {
        protected readonly IScene Scene;

        [Dependency]
        public ElementVisitor(IScene scene)
        {
            Scene = scene;
        }

        public virtual void VisitNode(Node node)
        {
        }

        public virtual void VisitWay(Way way)
        {
        }

        public virtual void VisitRelation(Relation relation)
        {
        }
    }

    /// <summary>
    /// Helper class which provides the way to use actions instead of subclassing
    /// </summary>
    public class ActionElementVisitor : IElementVisitor
    {
        private readonly Action<Node> _visitNode;
        private readonly Action<Relation> _visitRelation;
        private readonly Action<Way> _visitWay;

        public ActionElementVisitor(Action<Node> visitNode, Action<Way> visitWay, Action<Relation> visitRelation)
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
