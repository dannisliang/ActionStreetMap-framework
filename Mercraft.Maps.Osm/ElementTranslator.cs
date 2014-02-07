using System.Collections.Generic;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Models;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    /// Represents a style interpreter.
    /// </summary>
    public class ElementTranslator
    {
        private IElementVisitor _translateVisitor;

        public ElementTranslator(IElementVisitor translateVisitor)
        {
            _translateVisitor = translateVisitor;
        }

        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        public virtual void Translate(IScene scene, IDataSourceReadOnly source, IProjection projection, Element element)
        {
            element.Accept(new ElementVisitor(
                node => PopulateNode(node),
                way => PopulateWay(way, source),
                relation => PopulateRelation(relation, source)));

            element.Accept(_translateVisitor);
        }


        #region Populates given elements

        private Node PopulateNode(Node node)
        {
            node.Coordinate = new GeoCoordinate(node.Latitude.Value, node.Longitude.Value);
            return node;
        }

        private Way PopulateWay(Way way, IDataSourceReadOnly nodeSource)
        {
            int nodeCount = way.NodeIds.Count;
            way.Nodes = new List<Node>(nodeCount);
            for (int idx = 0; idx < nodeCount; idx++)
            {
                long nodeId = way.NodeIds[idx];
                Node node = nodeSource.GetNode(nodeId);
                if (node == null)
                    return null;
                PopulateNode(node);
                way.Nodes.Add(node);
            }

            return way;
        }

        private Relation PopulateRelation(Relation relation, IDataSourceReadOnly source)
        {
            var members = new List<RelationMember>(relation.Members.Count);

            for (int idx = 0; idx < relation.Members.Count; idx++)
            {
                RelationMember member = relation.Members[idx];
                long memberId = member.MemberId.Value;

                Element element = null;
                member.Member.Accept(new ElementVisitor(
                    _ => { element = source.GetNode(memberId); },
                    _ => { element = source.GetWay(memberId); },
                    _ => { element = source.GetRelation(memberId); }));

                if (element == null)
                    return null;

                member.Member = element;
                members.Add(member);
            }
            relation.Members = members;
            return relation;
        }

        #endregion
    }
}