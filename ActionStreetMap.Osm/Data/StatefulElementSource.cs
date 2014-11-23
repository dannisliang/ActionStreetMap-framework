using System;
using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Osm.Entities;
using ActionStreetMap.Osm.Visitors;

namespace ActionStreetMap.Osm.Data
{
    /// <summary>
    ///     An in-memory data repository of OSM data primitives.
    /// </summary>
    public abstract class StatefulElementSource : IElementSource
    {
        /// <summary>
        ///     Holds the current bounding box.
        /// </summary>
        private BoundingBox _box;

        /// <summary>
        ///     True if ready to use.
        /// </summary>
        protected bool IsInitialized { get; set; }

        /// <summary>
        ///     Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        protected StatefulElementSource()
        {
            InitializeDataStructures();
        }

        /// <summary>
        ///     Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        protected StatefulElementSource(params Element[] initial)
        {
            InitializeDataStructures();

            foreach (Element osmGeo in initial)
            {
                Add(osmGeo);
            }
        }

        /// <summary>
        ///     Initializes the data cache.
        /// </summary>
        private void InitializeDataStructures()
        {
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();

            _waysPerNode = new Dictionary<long, HashSet<long>>();

            _relationsPerNode = new Dictionary<long, HashSet<long>>();
            _relationsPerWay = new Dictionary<long, HashSet<long>>();
            _relationsPerRelation = new Dictionary<long, HashSet<long>>();
        }

        #region Objects Cache

        private Dictionary<long, Node> _nodes;

        private Dictionary<long, Way> _ways;

        private Dictionary<long, Relation> _relations;

        private Dictionary<long, HashSet<long>> _waysPerNode;

        private Dictionary<long, HashSet<long>> _relationsPerNode;

        private Dictionary<long, HashSet<long>> _relationsPerWay;

        private Dictionary<long, HashSet<long>> _relationsPerRelation;

        #endregion

        /// <summary>
        ///     Adds a new osmgeo object.
        /// </summary>
        public void Add(Element element)
        {
            element.Accept(new ActionElementVisitor(
                AddNode,
                AddWay,
                AddRelation));
        }

        /// <summary>
        ///     Returns the node with the given id.
        /// </summary>
        public Node GetNode(long id)
        {
            Node node;
            _nodes.TryGetValue(id, out node);
            return node;
        }

        /// <summary>
        ///     Returns the node(s) with the given id(s).
        /// </summary>
        public IList<Node> GetNodes(IList<long> ids)
        {
            List<Node> nodes = new List<Node>();
            if (ids != null)
            {
                // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    nodes.Add(GetNode(ids[idx]));
                }
            }
            return nodes;
        }

        /// <summary>
        ///     Returns all nodes in this memory datasource.
        /// </summary>
        public IEnumerable<Node> GetNodes()
        {
            return _nodes.Values;
        }

        /// <summary>
        ///     Adds a node.
        /// </summary>
        public void AddNode(Node node)
        {
            _nodes[node.Id] = node;

            if (_box == null)
            {
                _box = new BoundingBox(node.Coordinate, node.Coordinate);
            }
            else
            {
                _box = _box + node.Coordinate;
            }
        }

        /// <summary>
        ///     Removes a node.
        /// </summary>
        public void RemoveNode(long id)
        {
            _nodes.Remove(id);
        }

        /// <summary>
        ///     Returns the relation with the given id.
        /// </summary>
        public Relation GetRelation(long id)
        {
            Relation relation;
            _relations.TryGetValue(id, out relation);
            return relation;
        }

        /// <summary>
        ///     Returns the relation(s) with the given id(s).
        /// </summary>
        public IList<Relation> GetRelations(IList<long> ids)
        {
            List<Relation> relations = new List<Relation>();
            if (ids != null)
            {
                // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    relations.Add(GetRelation(ids[idx]));
                }
            }
            return relations;
        }

        /// <summary>
        ///     Returns all relations in this memory datasource.
        /// </summary>
        public IEnumerable<Relation> GetRelations()
        {
            return _relations.Values;
        }

        /// <summary>
        ///     Adds a relation.
        /// </summary>
        public void AddRelation(Relation relation)
        {
            if (relation == null) throw new ArgumentNullException();
            _relations[relation.Id] = relation;

            if (relation.Members != null)
            {
                foreach (var relationMember in relation.Members)
                {
                    HashSet<long> relationsIds = null;
                    RelationMember member = relationMember;
                    Action<Dictionary<long, HashSet<long>>> relationPerElementAction =
                        relationPerElement =>
                        {
                            long id = member.MemberId;
                            if (!relationPerElement.TryGetValue(id, out relationsIds))
                            {
                                relationsIds = new HashSet<long>();
                                relationPerElement.Add(id, relationsIds);
                            }
                        };

                    relationMember.Member.Accept(new ActionElementVisitor(
                        _ => relationPerElementAction(_relationsPerNode),
                        _ => relationPerElementAction(_relationsPerWay),
                        _ => relationPerElementAction(_relationsPerRelation)));

                    relationsIds.Add(relation.Id);
                }
            }
        }

        /// <summary>
        ///     Removes a relation.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveRelation(long id)
        {
            _relations.Remove(id);
        }

        /// <summary>
        ///     Returns all relations that have the given object as a member.
        /// </summary>
        public IList<Relation> GetRelationsFor(Element element)
        {
            long id = element.Id;
            HashSet<long> relationIds = null;

            element.Accept(new ActionElementVisitor(
                _ => _relationsPerNode.TryGetValue(id, out relationIds),
                _ => _relationsPerWay.TryGetValue(id, out relationIds),
                _ => _relationsPerRelation.TryGetValue(id, out relationIds)));

            if (relationIds == null)
                return new List<Relation>();

            var relations = new List<Relation>();
            foreach (long relationId in relationIds)
                relations.Add(GetRelation(relationId));

            return relations;
        }

        /// <summary>
        ///     Returns the way with the given id.
        /// </summary>
        public Way GetWay(long id)
        {
            Way way;
            _ways.TryGetValue(id, out way);
            return way;
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
        }

        /// <summary>
        ///     Returns all the way(s) with the given id(s).
        /// </summary>
        public IList<Way> GetWays(IList<long> ids)
        {
            List<Way> relations = new List<Way>();
            if (ids != null)
            {
                // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    relations.Add(GetWay(ids[idx]));
                }
            }
            return relations;
        }

        /// <summary>
        ///     Returns all ways in this memory datasource.
        /// </summary>
        public IEnumerable<Way> GetWays()
        {
            return _ways.Values;
        }

        /// <summary>
        ///     Returns all the ways for a given node.
        /// </summary>
        public IList<Way> GetWaysFor(long id)
        {
            HashSet<long> wayIds;
            List<Way> ways = new List<Way>();
            if (_waysPerNode.TryGetValue(id, out wayIds))
            {
                foreach (long wayId in wayIds)
                {
                    ways.Add(GetWay(wayId));
                }
            }
            return ways;
        }

        /// <summary>
        ///     Returns all ways containing one or more of the given ids.
        /// </summary>
        public IList<Way> GetWaysFor(IEnumerable<long> ids)
        {
            HashSet<long> allWayIds = new HashSet<long>();
            foreach (long id in ids)
            {
                HashSet<long> wayIds;
                if (_waysPerNode.TryGetValue(id, out wayIds))
                {
                    foreach (long wayId in wayIds)
                    {
                        allWayIds.Add(wayId);
                    }
                }
            }
            List<Way> ways = new List<Way>();
            foreach (long wayId in allWayIds)
            {
                ways.Add(GetWay(wayId));
            }
            return ways;
        }

        /// <summary>
        ///     Adds a way.
        /// </summary>
        public void AddWay(Way way)
        {
            if (way == null) throw new ArgumentNullException();

            _ways[way.Id] = way;

            if (way.NodeIds != null)
            {
                foreach (long nodeId in way.NodeIds)
                {
                    HashSet<long> wayIds;
                    if (!_waysPerNode.TryGetValue(nodeId, out wayIds))
                    {
                        wayIds = new HashSet<long>();
                        _waysPerNode.Add(nodeId, wayIds);
                    }
                    wayIds.Add(way.Id);
                }
            }
        }

        /// <summary>
        ///     Removes a way.
        /// </summary>
        public void RemoveWay(long id)
        {
            _ways.Remove(id);
        }

        /// <summary>
        ///     Returns all the objects within a given bounding box and filtered by a given filter.
        /// </summary>
        public virtual IEnumerable<Element> Get(BoundingBox bbox)
        {
            if (!IsInitialized)
                Initialize();

            List<Element> res = new List<Element>();

            // load all nodes and keep the ids in a collection.
            HashSet<long> ids = new HashSet<long>();
            foreach (Node node in _nodes.Values)
            {
                if (bbox.Contains(node.Coordinate))
                {
                    res.Add(node);
                    ids.Add(node.Id);
                }
            }

            // load all ways that contain the nodes that have been found.
            res.AddRange(GetWaysFor(ids).Cast<Element>());

            // get relations containing any of the nodes or ways in the current results-list.
            var relations = new List<Relation>();
            var relationIds = new HashSet<long>();
            foreach (Element osmGeo in res)
            {
                IList<Relation> relationsFor = GetRelationsFor(osmGeo);
                foreach (Relation relation in relationsFor)
                {
                    if (!relationIds.Contains(relation.Id))
                    {
                        relations.Add(relation);
                        relationIds.Add(relation.Id);
                    }
                }
            }

            // recursively add all relations containing other relations as a member.
            do
            {
                res.AddRange(relations.Cast<Element>()); // the .Cast<> is here for Windows Phone.
                var newRelations = new List<Relation>();
                foreach (Relation element in relations)
                {
                    IList<Relation> relationsFor = GetRelationsFor(element);
                    foreach (Relation relation in relationsFor)
                    {
                        if (!relationIds.Contains(relation.Id))
                        {
                            newRelations.Add(relation);
                            relationIds.Add(relation.Id);
                        }
                    }
                }
                relations = newRelations;
            } while (relations.Count > 0);

            return res;
        }

        /// <inheritdoc />
        public virtual void Initialize()
        {
            if (!IsInitialized)
                throw new InvalidOperationException(
                    "You should call other Initialize(IEnumerable<Element>) method before this one");
        }

        /// <inheritdoc />
        public void Initialize(IEnumerable<Element> sourceStream)
        {
            var elementVisitor = new ActionElementVisitor(
                AddNode,
                AddWay,
                AddRelation);

            foreach (var element in sourceStream)
                element.Accept(elementVisitor);
            IsInitialized = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}