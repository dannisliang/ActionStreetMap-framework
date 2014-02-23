using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Filters;
using Mercraft.Maps.Osm.Format.Xml.v0_6;
using Mercraft.Maps.Osm.Formats.Pbf;
using Mercraft.Maps.Osm.Streams;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;
using Node = Mercraft.Maps.Osm.Entities.Node;
using Relation = Mercraft.Maps.Osm.Entities.Relation;
using Way = Mercraft.Maps.Osm.Entities.Way;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// An in-memory data repository of OSM data primitives.
    /// </summary>
    public class MemoryDataSource : DataSourceReadOnlyBase
    {
        /// <summary>
        /// Holds the current bounding box.
        /// </summary>
        private BoundingBox _box = null;

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemoryDataSource()
        {
            this.InitializeDataStructures();
        }

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemoryDataSource(params Element[] initial)
        {
            this.InitializeDataStructures();

            foreach (Element osmGeo in initial)
            {
                this.Add(osmGeo);
            }
        }

        /// <summary>
        /// Initializes the data cache.
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
        /// Adds a new osmgeo object.
        /// </summary>
        public void Add(Element element)
        {
            element.Accept(new ElementVisitor(
                AddNode,
                AddWay,
                AddRelation));
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        public override Node GetNode(long id)
        {
            Node node = null;
            _nodes.TryGetValue(id, out node);
            return node;
        }

        /// <summary>
        /// Returns the node(s) with the given id(s).
        /// </summary>
        public override IList<Node> GetNodes(IList<long> ids)
        {
            List<Node> nodes = new List<Node>();
            if (ids != null)
            { // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    nodes.Add(this.GetNode(ids[idx]));
                }
            }
            return nodes;
        }

        /// <summary>
        /// Returns all nodes in this memory datasource.
        /// </summary>
        public IEnumerable<Node> GetNodes()
        {
            return _nodes.Values;
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        public void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException();
            if (!node.Id.HasValue) throw new ArgumentException("NodeIds without a valid id cannot be saved!");
            if (!node.Latitude.HasValue || !node.Longitude.HasValue) throw new ArgumentException("NodeIds without a valid longitude/latitude pair cannot be saved!");

            _nodes[node.Id.Value] = node;

            if (_box == null)
            {
                _box = new BoundingBox(new GeoCoordinate(node.Latitude.Value, node.Longitude.Value),
                    new GeoCoordinate(node.Latitude.Value, node.Longitude.Value));
            }
            else
            {
                _box = _box + new GeoCoordinate(node.Latitude.Value, node.Longitude.Value);
            }
        }

        /// <summary>
        /// Removes a node.
        /// </summary>
        public void RemoveNode(long id)
        {
            _nodes.Remove(id);
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        public override Relation GetRelation(long id)
        {
            Relation relation = null;
            _relations.TryGetValue(id, out relation);
            return relation;
        }

        /// <summary>
        /// Returns the relation(s) with the given id(s).
        /// </summary>
        public override IList<Relation> GetRelations(IList<long> ids)
        {
            List<Relation> relations = new List<Relation>();
            if (ids != null)
            { // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    relations.Add(this.GetRelation(ids[idx]));
                }
            }
            return relations;
        }

        /// <summary>
        /// Returns all relations in this memory datasource.
        /// </summary>
        public IEnumerable<Relation> GetRelations()
        {
            return _relations.Values;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        public void AddRelation(Relation relation)
        {
            if (relation == null) throw new ArgumentNullException();
            if (!relation.Id.HasValue) throw new ArgumentException("Relations without a valid id cannot be saved!");

            _relations[relation.Id.Value] = relation;

            if (relation.Members != null)
            {
                foreach (var relationMember in relation.Members)
                {
                    HashSet<long> relationsIds = null;
                    Action<Dictionary<long, HashSet<long>>> relationPerElementAction = 
                        relationPerElement =>
                    {
                        long id = relationMember.MemberId.Value;
                        if (!relationPerElement.TryGetValue(id, out relationsIds))
                        {
                            relationsIds = new HashSet<long>();
                            relationPerElement.Add(id, relationsIds);
                        }
                    };

                    relationMember.Member.Accept(new ElementVisitor(
                       _ => relationPerElementAction(_relationsPerNode),
                       _ => relationPerElementAction(_relationsPerWay),
                       _ => relationPerElementAction(_relationsPerRelation)));

                    relationsIds.Add(relation.Id.Value);
                }
            }
        }

        /// <summary>
        /// Removes a relation.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveRelation(long id)
        {
            _relations.Remove(id);
        }

        /// <summary>
        /// Returns all relations that have the given object as a member.
        /// </summary>
        public override IList<Relation> GetRelationsFor(Element element)
        {
            long id = element.Id.Value;
            HashSet<long> relationIds = null;
            
            element.Accept(new ElementVisitor(
                _ => { _relationsPerNode.TryGetValue(id, out relationIds); },
                _ => { _relationsPerWay.TryGetValue(id, out relationIds); },
                _ => { _relationsPerRelation.TryGetValue(id, out relationIds); }));

            if (relationIds == null)
                return new List<Relation>(); 

            var relations = new List<Relation>();
            foreach (long relationId in relationIds)
                relations.Add(GetRelation(relationId));

            return relations;
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        public override Way GetWay(long id)
        {
            Way way = null;
            _ways.TryGetValue(id, out way);
            return way;
        }

        /// <summary>
        /// Returns all the way(s) with the given id(s).
        /// </summary>
        public override IList<Way> GetWays(IList<long> ids)
        {
            List<Way> relations = new List<Way>();
            if (ids != null)
            { // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    relations.Add(this.GetWay(ids[idx]));
                }
            }
            return relations;
        }

        /// <summary>
        /// Returns all ways in this memory datasource.
        /// </summary>
        public IEnumerable<Way> GetWays()
        {
            return _ways.Values;
        }

        /// <summary>
        /// Returns all the ways for a given node.
        /// </summary>
        public override IList<Way> GetWaysFor(long id)
        {
            HashSet<long> wayIds = null;
            List<Way> ways = new List<Way>();
            if (_waysPerNode.TryGetValue(id, out wayIds))
            {
                foreach (long wayId in wayIds)
                {
                    ways.Add(this.GetWay(wayId));
                }
            }
            return ways;
        }

        /// <summary>
        /// Returns all ways containing one or more of the given ids.
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
                ways.Add(this.GetWay(wayId));
            }
            return ways;
        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        public void AddWay(Way way)
        {
            if (way == null) throw new ArgumentNullException();
            if (!way.Id.HasValue) throw new ArgumentException("Ways without a valid id cannot be saved!");

            _ways[way.Id.Value] = way;

            if(way.NodeIds != null)
            {
                foreach(long nodeId in way.NodeIds)
                {
                    HashSet<long> wayIds;
                    if (!_waysPerNode.TryGetValue(nodeId, out wayIds))
                    {
                        wayIds = new HashSet<long>();
                        _waysPerNode.Add(nodeId, wayIds);
                    }
                    wayIds.Add(way.Id.Value);
                }
            }
        }

        /// <summary>
        /// Removes a way.
        /// </summary>
        public void RemoveWay(long id)
        {
            _ways.Remove(id);
        }

        /// <summary>
        /// Returns all the objects within a given bounding box and filtered by a given filter.
        /// </summary>
        public override IList<Element> Get(BoundingBox bbox, IFilter filter)
        {
            List<Element> res = new List<Element>();

            // load all nodes and keep the ids in a collection.
            HashSet<long> ids = new HashSet<long>();
            foreach (Node node in _nodes.Values)
            {
                if ((filter == null || filter.Evaluate(node)) &&
                    bbox.Contains(node.Latitude.Value, node.Longitude.Value))
                {
                    res.Add(node);
                    ids.Add(node.Id.Value);
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
                    if (!relationIds.Contains(relation.Id.Value))
                    {
                        relations.Add(relation);
                        relationIds.Add(relation.Id.Value);
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
                        if (!relationIds.Contains(relation.Id.Value))
                        {
                            newRelations.Add(relation);
                            relationIds.Add(relation.Id.Value);
                        }
                    }
                }
                relations = newRelations;
            } while (relations.Count > 0);

            if (filter != null)
            {
                var filtered = new List<Element>();
                foreach (Element geo in res)
                {
                    if (filter.Evaluate(geo))
                    {
                        filtered.Add(geo);
                    }
                }
            }

            return res;
        }

        #region Create Functions

        /// <summary>
        /// Creates a new memory data source from all the data in the given osm-stream.
        /// </summary>
        public static MemoryDataSource CreateFrom(OsmStreamSource sourceStream)
        {
            // reset if possible.
            if (sourceStream.CanReset)
                sourceStream.Reset();

            // enumerate all objects and add them to a new datasource.
            var dataSource = new MemoryDataSource();

            var elementVisitor = new ElementVisitor(
                dataSource.AddNode,
                dataSource.AddWay,
                dataSource.AddRelation);

            foreach (var element in sourceStream)
                element.Accept(elementVisitor);

            return dataSource;
        }

        /// <summary>
        /// Creates a new memory data source from all the data in the given osm pbf stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFromPbfStream(Stream stream)
        {
            return MemoryDataSource.CreateFrom(new PbfOsmStreamSource(stream));
        }

        public static MemoryDataSource CreateFromXmlStream(Stream stream)
        {
            return MemoryDataSource.CreateFrom(new Mercraft.Maps.Osm.Formats.Xml.XmlOsmStreamSource(stream));
        }

        #endregion
    }
}