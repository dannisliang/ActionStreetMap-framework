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
    public abstract class ElementTranslator
    {
        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="source"></param>
        /// <param name="element"></param>
        public virtual void Translate(IScene scene, IDataSourceReadOnly source, IProjection projection, Element element)
        {
            switch (element.Type)
            {
                case ElementType.Node:
                    this.Translate(scene, projection, PopulateNode(element as Node));
                    break;
                case ElementType.Way:
                    this.Translate(scene, projection, PopulateWay(element as Way, source));
                    break;
                case ElementType.Relation:
                    this.Translate(scene, projection, PopulateRelation(element as Relation, source));
                    break;
            }
        }

        /// <summary>
        /// Translates the given Node into corresponding geometries.
        /// </summary>
        public abstract void Translate(IScene scene, IProjection projection, Node node);

        /// <summary>
        /// Translates the given Way into corresponding geometries.
        /// </summary>
        public abstract void Translate(IScene scene, IProjection projection, Way way);

        /// <summary>
        /// Translates the given Relation into corresponding geometries.
        /// </summary>
        public abstract void Translate(IScene scene, IProjection projection, Relation relation);

        /// <summary>
        /// Returns true if this style applies to the given object.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract bool AppliesTo(Element element);


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
            List<RelationMember> members = new List<RelationMember>(relation.Members.Count);
            for (int idx = 0; idx < relation.Members.Count; idx++)
            {
                RelationMember member = relation.Members[idx];
                long memberId = member.MemberId.Value;
                switch (relation.Members[idx].MemberType.Value)
                {
                    case ElementType.Node:
                        Node node = source.GetNode(memberId);
                        if (node == null)
                            return null;
                        member.Member = node;
                        break;
                    case ElementType.Way:
                        Way way = source.GetWay(memberId);
                        if (way == null)
                            return null;
                        member.Member = way;
                        break;
                    case ElementType.Relation:
                        Relation relationMember = source.GetRelation(memberId);
                        if (relationMember == null)
                            return null;
                        member.Member = relationMember;
                        break;
                }
                members.Add(member);
            }
            relation.Members = members;
            return relation;
        }

        #endregion
    }
}