using System.Collections.Generic;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Formats;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    /// Manages elements in bbox
    /// </summary>
    public class ElementManager
    {
        /// <summary>
        /// Visits all elements in datasource which are located in bbox
        /// </summary>
        public void VisitBoundingBox(BoundingBox bbox, IElementSource elementSource,  IElementVisitor visitor)
        {
            IEnumerable<Element> elements = elementSource.Get(bbox);
            foreach (var element in elements)
            { 
               Populate(element, elementSource);
               element.Accept(visitor); 
            }
        }

        /// <summary>
        /// Populates the given OSM objects into corresponding geometries.
        /// </summary>
        private void Populate(Element element, IElementSource elementSource)
        {
            element.Accept(new ElementVisitor(
                node => PopulateNode(node),
                way => PopulateWay(way, elementSource),
                relation => PopulateRelation(relation, elementSource)));
 
        }

        #region Populates given elements

        private Node PopulateNode(Node node)
        {
            node.Coordinate = new GeoCoordinate(node.Latitude.Value, node.Longitude.Value);
            return node;
        }

        private Way PopulateWay(Way way, IElementSource elementSource)
        {
            int nodeCount = way.NodeIds.Count;
            way.Nodes = new List<Node>(nodeCount);
            for (int idx = 0; idx < nodeCount; idx++)
            {
                long nodeId = way.NodeIds[idx];
                Node node = elementSource.GetNode(nodeId);
                if (node == null)
                    return null;
                PopulateNode(node);
                way.Nodes.Add(node);
            }

            return way;
        }

        private Relation PopulateRelation(Relation relation, IElementSource elementSource)
        {
            var members = new List<RelationMember>(relation.Members.Count);

            for (int idx = 0; idx < relation.Members.Count; idx++)
            {
                RelationMember member = relation.Members[idx];
                long memberId = member.MemberId.Value;

                Element element = null;
                member.Member.Accept(new ElementVisitor(
                    _ => { element = elementSource.GetNode(memberId); },
                    _ => { element = elementSource.GetWay(memberId); },
                    _ => { element = elementSource.GetRelation(memberId); }));

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
