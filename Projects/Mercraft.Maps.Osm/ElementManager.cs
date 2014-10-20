using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Infrastructure.Primitives;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Visitors;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    ///     Manages elements in bbox. Stateful class (not thread safe!)
    /// </summary>
    public class ElementManager : IElementVisitor
    {
        private IElementSource _currentElementSource;
        private BoundingBox _currentBoundingBox;

        private readonly Dictionary<long, Node> _unresolvedNodes = new Dictionary<long, Node>();

        /// <summary>
        ///     Stores ways which crosses border between tiles
        ///     Key: way id
        ///     Value: Tuple of way instance and boolean flag which true if we added way in current request
        /// </summary>
        private readonly Dictionary<long, Tuple<Way, bool>> _crossTileWays = new Dictionary<long, Tuple<Way, bool>>();

        private readonly List<long> _keysToDelete = new List<long>(64);

        /// <summary>
        ///     Visits all elements in datasource which are located in bbox.
        /// </summary>
        public void VisitBoundingBox(BoundingBox bbox, IElementSource elementSource, IElementVisitor visitor)
        {
            // needed by IElementVisitor methods of this
            _currentElementSource = elementSource;
            _currentBoundingBox = bbox;

            // process elements from elements source
            IEnumerable<Element> elements = elementSource.Get(bbox);
            foreach (var element in elements)
            {
                element.Accept(this);
                element.Accept(visitor);
            }
            elementSource.Reset();

            ProcessLeftovers(bbox, visitor);
        }

        #region IElementVisitor implementation

        /// <inheritdoc />
        public void VisitNode(Node node)
        {
            // Do nothing
        }

        /// <inheritdoc />
        public void VisitWay(Way way)
        {
            PopulateWay(_currentBoundingBox, way, _currentElementSource);
        }

        /// <inheritdoc />
        public void VisitRelation(Relation relation)
        {
            PopulateRelation(relation, _currentElementSource);
        }

        #endregion

        #region Populates given elements

        private void PopulateWay(BoundingBox bbox, Way way, IElementSource elementSource)
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
                //PopulateNode(node);
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
            }
            else
                CheckOutOfBoxNodes(bbox, way);
        }

        private void PopulateRelation(Relation relation, IElementSource elementSource)
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
        }

        #endregion

        private void ProcessLeftovers(BoundingBox bbox, IElementVisitor visitor)
        {
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
                        _keysToDelete.Add(crossTileWay.Key);
                }
                else
                    _keysToDelete.Add(crossTileWay.Key);
            }
            // we should cleanup way which has no nodes with IsOutOfBox = true;
            _keysToDelete.ForEach(k => _crossTileWays.Remove(k));
            _keysToDelete.Clear();
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