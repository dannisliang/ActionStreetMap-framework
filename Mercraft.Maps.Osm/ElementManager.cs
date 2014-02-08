using System.Collections.Generic;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Extensions.LongIndex;
using Mercraft.Maps.Osm.Filters;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    /// Manages elements in bbox
    /// </summary>
    public class ElementManager
    {
        private IElementVisitor _elementVisitor;

        /// <summary>
        /// Holds the interpreted nodes.
        /// </summary>
        private LongIndex _translatedNodes;

        /// <summary>
        /// Holds the interpreted relations.
        /// </summary>
        private LongIndex _translatedRelations;

        /// <summary>
        /// Holds the interpreted way.
        /// </summary>
        private LongIndex _translatedWays;


        /// <summary>
        /// Creates a new style scene manager.
        /// </summary>
        public ElementManager(IElementVisitor elementVisitor)
        {
            _elementVisitor = elementVisitor;

            _translatedNodes = new LongIndex();
            _translatedWays = new LongIndex();
            _translatedRelations = new LongIndex();
        }

        public void FillScene(IDataSourceReadOnly dataSource, BoundingBox box, IFilter filter = null)
        {
            IList<Element> elements = dataSource.Get(box, filter);
            foreach (var element in elements)
            { // translate each object into scene object.
                LongIndex index = null;

                element.Accept(new ElementVisitor(
                    _ => index = _translatedNodes,
                    _ => index = _translatedWays,
                    _ => index = _translatedRelations));
               
                if (!index.Contains(element.Id.Value))
                {
                    // object was not yet interpreted.
                    index.Add(element.Id.Value);
                    Populate(dataSource, element);
                }
            }
        }

        /// <summary>
        /// Populates the given OSM objects into corresponding geometries.
        /// </summary>
        private void Populate(IDataSourceReadOnly source, Element element)
        {
            element.Accept(new ElementVisitor(
                node => PopulateNode(node),
                way => PopulateWay(way, source),
                relation => PopulateRelation(relation, source)));

            element.Accept(_elementVisitor);
        }

        #region Populates given elements

        private Node PopulateNode(Node node)
        {
            node.Coordinate = new MapPoint(node.Latitude.Value, node.Longitude.Value);
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
