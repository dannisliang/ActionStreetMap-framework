using System.Collections.Generic;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Infrastructure.Primitives;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    /// Manages elements in bbox. Stateful class (not thread safe!)
    /// </summary>
    public class ElementManager
    {
        private Dictionary<long, Node> _unresolvedNodes = new Dictionary<long, Node>();

        /// <summary>
        /// Stores ways which crosses border between tiles
        /// Key: way id
        /// Value: Tuple of way instance and boolean flag which true if we added way in current request
        /// </summary>
        private Dictionary<long, Tuple<Way, bool>> _crossTileWays = new Dictionary<long, Tuple<Way, bool>>();

        /// <summary>
        /// Visits all elements in datasource which are located in bbox
        /// </summary>
        public void VisitBoundingBox(BoundingBox bbox, IElementSource elementSource,  IElementVisitor visitor)
        {
            // process elements from elements source
            IEnumerable<Element> elements = elementSource.Get(bbox);
            foreach (var element in elements)
            {
                Populate(bbox, element, elementSource);
                element.Accept(visitor);
            }

            ProcessLeftovers(bbox, visitor);
        }

        /// <summary>
        /// Populates the given OSM objects into corresponding geometries.
        /// </summary>
        private void Populate(BoundingBox bbox, Element element, IElementSource elementSource)
        {
            element.Accept(new ActionElementVisitor(
                node => PopulateNode(node),
                way => PopulateWay(bbox, way, elementSource),
                relation => PopulateRelation(relation, elementSource)));
        }

        #region Populates given elements

        private Node PopulateNode(Node node)
        {
            return node;
        }

        private Way PopulateWay(BoundingBox bbox, Way way, IElementSource elementSource)
        {
            int nodeCount = way.NodeIds.Count;
            way.Nodes = new List<Node>(nodeCount);
            var latestIndex = nodeCount - 1;
            for (int idx = 0; idx <= latestIndex; idx++)
            {
                long nodeId = way.NodeIds[idx];
                Node node = elementSource.GetNode(nodeId);

                // NOTE As result, we need sort nodes in the same order like nodeIds property before create polygon!
                if (node == null)
                {
                    // try to resolve in HashSet
                    if (_unresolvedNodes.ContainsKey(nodeId))
                    {
                        node = _unresolvedNodes[nodeId];
                        // this is necessary due to fact that first and last items the same
                        if (idx == latestIndex && way.NodeIds[0] == way.NodeIds[latestIndex])
                            _unresolvedNodes.Remove(nodeId);
                    }
                    else
                    {
                        continue;
                    }
                }
                PopulateNode(node);
                way.Nodes.Add(node);
            }

            if (way.Nodes.Count != way.NodeIds.Count)
            {
                // way with any unresolved node is useless
                // memorize all resolved nodes and wait for next bbox request
                // to resolve the rest
                foreach (var node in way.Nodes)
                {
                    if (!_unresolvedNodes.ContainsKey(node.Id))
                    {
                        _unresolvedNodes.Add(node.Id, node);
                        if (!_crossTileWays.ContainsKey(way.Id))
                            _crossTileWays.Add(way.Id, new Tuple<Way, bool>(way, true));
                    }
                }
                return null;
            }

            CheckOutOfBoxNodes(bbox, way);
            return way;
        }

        private Relation PopulateRelation(Relation relation, IElementSource elementSource)
        {
            var members = new List<RelationMember>(relation.Members.Count);

            for (int idx = 0; idx < relation.Members.Count; idx++)
            {
                RelationMember member = relation.Members[idx];
                long memberId = member.MemberId;

                Element element = null;
                member.Member.Accept(new ActionElementVisitor(
                    _ => { element = elementSource.GetNode(memberId); },
                    _ => { element = elementSource.GetWay(memberId); },
                    _ => { element = elementSource.GetRelation(memberId); }));

                if (element == null) continue;

                member.Member = element;
                members.Add(member);
            }
            relation.Members = members;
            return relation;
        }

        #endregion


        private void ProcessLeftovers(BoundingBox bbox, IElementVisitor visitor)
        {
            List<long> keysToDelete = new List<long>();
            // process elements (ways) which cross tile borders
            foreach (var crossTileWay in _crossTileWays)
            {
                var crossTileWayTuple = crossTileWay.Value;

                // skip it as already processed (above)
                if (crossTileWayTuple.Item2)
                {
                    // make it availabe for future processing for different tiles
                    crossTileWayTuple.Item2 = false;
                    continue;
                }

                var way = crossTileWayTuple.Item1;

                // test all nodes to find any which is located in given bbox
                // which means that we should process this way also
                bool isInBbox = false;
                bool hasOutOfBoxNotProcessed = false;
                foreach (var node in way.Nodes)
                {
                    // IsOutOfBox in this context means that this node was out of 
                    // bounding box for request where it was created
                    if (node.IsOutOfBox)
                    {
                        if (bbox.Contains(node.Coordinate))
                        {
                            // mark it as processed
                            node.IsOutOfBox = false;
                            isInBbox = true;
                        }
                        else
                        {
                            hasOutOfBoxNotProcessed = true;
                        }
                    }

                }
                if (isInBbox)
                {
                    way.Accept(visitor);
                    if (!hasOutOfBoxNotProcessed)
                        keysToDelete.Add(crossTileWay.Key);
                }
                else
                    keysToDelete.Add(crossTileWay.Key);
            }
            // we should cleanup way which has no nodes with IsOutOfBox = true;
            keysToDelete.ForEach(k => _crossTileWays.Remove(k));
        }

        private void CheckOutOfBoxNodes(BoundingBox bbox, Way way)
        {
            if (_crossTileWays.ContainsKey(way.Id))
                return;
            for (int i = 0; i < way.Nodes.Count; i++)
            {
                // NOTE should we check bbox for real element source?
                if (way.Nodes[i].IsOutOfBox /* && !bbox.Contains(way.Nodes[i].Coordinate)*/)
                {
                    // TODO should we check existence?
                    _crossTileWays.Add(way.Id, new Tuple<Way, bool>(way, true));
                    return;
                }
            }
        }

    }
}
