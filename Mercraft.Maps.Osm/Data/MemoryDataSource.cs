using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercraft.Maps.Core;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Filters;
using Mercraft.Maps.Osm.Pbf;
using Mercraft.Maps.Osm.Streams;
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
            _id = Guid.NewGuid(); // creates a new Guid.

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
        /// Holds the current bounding box.
        /// </summary>
        private GeoCoordinateBox _box = null;

        /// <summary>
        /// Returns the bounding box around all nodes.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get { return _box; }
        }

        /// <summary>
        /// Gets/Sets the name of this source.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Holds the current Guid.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Returns the id.
        /// </summary>
        public override Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Returns true if there is a bounding box.
        /// </summary>
        public override bool HasBoundinBox
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if this source is readonly.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Adds a new osmgeo object.
        /// </summary>
        /// <param name="element"></param>
        public void Add(Element element)
        {
            if (element is Node)
            {
                this.AddNode(element as Node);
            }
            else if (element is Way)
            {
                this.AddWay(element as Way);
            }
            else if (element is Relation)
            {
                this.AddRelation(element as Relation);
            }
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Node GetNode(long id)
        {
            Node node = null;
            _nodes.TryGetValue(id, out node);
            return node;
        }

        /// <summary>
        /// Returns the node(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IEnumerable<Node> GetNodes()
        {
            return _nodes.Values;
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException();
            if (!node.Id.HasValue) throw new ArgumentException("NodeIds without a valid id cannot be saved!");
            if (!node.Latitude.HasValue || !node.Longitude.HasValue) throw new ArgumentException("NodeIds without a valid longitude/latitude pair cannot be saved!");

            _nodes[node.Id.Value] = node;

            if (_box == null)
            {
                _box = new GeoCoordinateBox(new GeoCoordinate(node.Latitude.Value, node.Longitude.Value),
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
        /// <param name="id"></param>
        public void RemoveNode(long id)
        {
            _nodes.Remove(id);
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Relation GetRelation(long id)
        {
            Relation relation = null;
            _relations.TryGetValue(id, out relation);
            return relation;
        }

        /// <summary>
        /// Returns the relation(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IEnumerable<Relation> GetRelations()
        {
            return _relations.Values;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public void AddRelation(Relation relation)
        {
            if (relation == null) throw new ArgumentNullException();
            if (!relation.Id.HasValue) throw new ArgumentException("Relations without a valid id cannot be saved!");

            _relations[relation.Id.Value] = relation;

            if (relation.Members != null)
            {
                foreach (var member in relation.Members)
                {
                    HashSet<long> relationsIds = null;
                    switch(member.MemberType.Value)
                    {
                        case ElementType.Node:
                            if (!_relationsPerNode.TryGetValue(member.MemberId.Value, out relationsIds))
                            {
                                relationsIds = new HashSet<long>();
                                _relationsPerNode.Add(member.MemberId.Value, relationsIds);
                            }
                            break;
                        case ElementType.Way:
                            if (!_relationsPerWay.TryGetValue(member.MemberId.Value, out relationsIds))
                            {
                                relationsIds = new HashSet<long>();
                                _relationsPerWay.Add(member.MemberId.Value, relationsIds);
                            }
                            break;
                        case ElementType.Relation:
                            if (!_relationsPerRelation.TryGetValue(member.MemberId.Value, out relationsIds))
                            {
                                relationsIds = new HashSet<long>();
                                _relationsPerRelation.Add(member.MemberId.Value, relationsIds);
                            }
                            break;
                    }
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
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelationsFor(ElementType type, long id)
        {
            List<Relation> relations = new List<Relation>();
            HashSet<long> relationIds = null;
            switch(type)
            {
                case ElementType.Node:
                    if (!_relationsPerNode.TryGetValue(id, out relationIds))
                    {
                        return relations;
                    }
                    break;
                case ElementType.Way:
                    if (!_relationsPerWay.TryGetValue(id, out relationIds))
                    {
                        return relations;
                    }
                    break;
                case ElementType.Relation:
                    if (!_relationsPerRelation.TryGetValue(id, out relationIds))
                    {
                        return relations;
                    }
                    break;
            }
            foreach (long relationId in relationIds)
            {
                relations.Add(this.GetRelation(relationId));
            }
            return relations;
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Way GetWay(long id)
        {
            Way way = null;
            _ways.TryGetValue(id, out way);
            return way;
        }

        /// <summary>
        /// Returns all the way(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IEnumerable<Way> GetWays()
        {
            return _ways.Values;
        }

        /// <summary>
        /// Returns all the ways for a given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// Returns all ways containing one or more of the given idx.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        /// <param name="way"></param>
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
        /// <param name="id"></param>
        public void RemoveWay(long id)
        {
            _ways.Remove(id);
        }

        /// <summary>
        /// Returns all the objects within a given bounding box and filtered by a given filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IList<Element> Get(GeoCoordinateBox box, IFilter filter)
        {
            List<Element> res = new List<Element>();

            // load all nodes and keep the ids in a collection.
            HashSet<long> ids = new HashSet<long>();
            foreach (Node node in _nodes.Values)
            {
                if ((filter == null || filter.Evaluate(node)) && 
                    box.Contains(new GeoCoordinate(node.Latitude.Value, node.Longitude.Value)))
                {
                    res.Add(node);
                    ids.Add(node.Id.Value);
                }
            }

            // load all ways that contain the nodes that have been found.
            res.AddRange(this.GetWaysFor(ids).Cast<Element>()); // the .Cast<> is here for Windows Phone.

            // get relations containing any of the nodes or ways in the current results-list.
            List<Relation> relations = new List<Relation>();
            HashSet<long> relationIds = new HashSet<long>();
            foreach (Element osmGeo in res)
            {
                IList<Relation> relationsFor = this.GetRelationsFor(osmGeo);
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
                List<Relation> newRelations = new List<Relation>();
                foreach (Element osmGeo in relations)
                {
                    IList<Relation> relationsFor = this.GetRelationsFor(osmGeo);
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
                List<Element> filtered = new List<Element>();
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
        /// <param name="sourceStream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFrom(OsmStreamSource sourceStream)
        {
            // reset if possible.
            if (sourceStream.CanReset) { sourceStream.Reset(); }

            // enumerate all objects and add them to a new datasource.
            MemoryDataSource dataSource = new MemoryDataSource();
            foreach (var osmGeo in sourceStream)
            {
                if (osmGeo != null)
                {
                    switch(osmGeo.Type)
                    {
                        case ElementType.Node:
                            dataSource.AddNode(osmGeo as Node);
                            break;
                        case ElementType.Way:
                            dataSource.AddWay(osmGeo as Way);
                            break;
                        case ElementType.Relation:
                            dataSource.AddRelation(osmGeo as Relation);
                            break;
                    }
                }
            }
            return dataSource;
        }

        /*
#if !WINDOWS_PHONE
        /// <summary>
        /// Creates a new memory data source from all the data in the given osm xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFromXmlStream(Stream stream)
        {
            return MemoryDataSource.CreateFrom(new OsmSharp.Osm.Xml.Streams.XmlOsmStreamSource(stream));
        }
#endif*/

        /// <summary>
        /// Creates a new memory data source from all the data in the given osm pbf stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFromPBFStream(Stream stream)
        {
            return MemoryDataSource.CreateFrom(new PbfOsmStreamSource(stream));
        }

        #endregion
    }
}