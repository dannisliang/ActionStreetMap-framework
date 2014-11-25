using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ActionStreetMap.Core;
using ActionStreetMap.Infrastructure.Utilities;
using ActionStreetMap.Osm.Entities;
using ActionStreetMap.Osm.Formats.Pbf;

namespace ActionStreetMap.Osm.Data
{
    /// <summary>
    ///     Pbf element source.
    /// </summary>
    public class PbfElementSource : IElementSource
    {
        /// <summary>
        ///     Holds the Pbf reader.
        /// </summary>
        private readonly PbfReader _reader;

        /// <summary>
        ///     Holds the source of the data.
        /// </summary>
        private Stream _stream;

        private readonly HashSet<long> _nodeIds = new HashSet<long>();
        private readonly HashSet<long> _wayIds = new HashSet<long>();
        private readonly HashSet<long> _relationIds = new HashSet<long>();

        private readonly HashSet<long> _unresolvedNodes = new HashSet<long>();

        /// <summary>
        ///     Elements dictionary.
        /// </summary>
        protected Dictionary<long, Element> Elements;

        /// <summary>
        ///     Creates PbfElementSource.
        /// </summary>
        protected PbfElementSource()
        {
            _reader = new PbfReader();
            Elements = new Dictionary<long, Element>(4096);
        }

        /// <summary>
        ///     Creates a new source of Pbf formated OSM data.
        /// </summary>
        public PbfElementSource(Stream stream): this()
        {
            SetStream(stream);
        }

        /// <summary>
        ///     Sets inner stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        protected void SetStream(Stream stream)
        {
            _stream = stream;
            _reader.SetStream(stream);
            ResetProcessedIds();
        }

        /// <inheritdoc />
        public virtual IEnumerable<Element> GetElements()
        {
            return Elements.Values;
        }

        /// <inheritdoc />
        public virtual IEnumerable<Element> Get(BoundingBox bbox)
        {
            FillElements(bbox);
            ResetProcessedIds();
            ResetUnresolvedId();
            return Elements.Values;
        }

        /// <summary>
        ///     Resets processed id collections.
        /// </summary>
        protected void ResetProcessedIds()
        {           
            _nodeIds.Clear();
            _wayIds.Clear();
            _relationIds.Clear();
        }

        /// <summary>
        ///     Resets unresolved id.
        /// </summary>
        protected void ResetUnresolvedId()
        {
            _unresolvedNodes.Clear();
        }

        #region Fill elements collection from pbf stream logic

        /// <summary>
        ///     Fills Elements collection with elements located in bounding box, but with undersolved references.
        /// </summary>
        protected void FillElements(BoundingBox bbox)
        {
            while (_reader.MoveNext())
            {
                var block = _reader.Current;
                var obbox = new OffsetBoundingBox(bbox, block);
                ProcessPrimitiveBlock(block, obbox);

                foreach (var primitiveGroup in block.primitivegroup)
                {
                    if (!primitiveGroup.IsNodeListEmpty)
                    {
                        foreach (var node in primitiveGroup.nodes)
                        {
                            // check bbox
                            //if (obbox.Contains(node.lat, node.lon))
                            SearchNode(block, node);
                        }
                    }

                    if (!primitiveGroup.IsWayListEmpty)
                    {
                        foreach (var way in primitiveGroup.ways)
                        {
                            SearchWay(block, way);
                        }
                    }

                    if (!primitiveGroup.IsRelationListEmpty)
                    {
                        foreach (var relation in primitiveGroup.relations)
                        {
                            SearchRelation(block, relation);
                        }
                    }
                }
            }

            // Resolve unresolved nodes
            // NOTE We assume that we slit large pbf files to multiply smaller ones with single
            // index file and PbfReader cached deserialized blocks as we use its caching methods
            
            _reader.Reset();

            while (_reader.MoveNext())
            {
                var block = _reader.Current;
                // NOTE take a look at second parameter: it's null and this is 
                // performance optimization to filter out nodes as early as possible
                ProcessPrimitiveBlock(block, null);
                foreach (var primitiveGroup in block.primitivegroup)
                {
                    if (primitiveGroup.nodes != null)
                    {
                        foreach (var node in primitiveGroup.nodes)
                        {
                            SearchNode(block, node);
                        }
                    }
                }
            }
            _reader.Reset();
        }

        private void SearchNode(PrimitiveBlock block, Formats.Pbf.Node node)
        {
            var latitude = .000000001*(block.lat_offset + (block.granularity*(double) node.lat));
            var longitude = .000000001*(block.lon_offset + (block.granularity*(double) node.lon));

            var elementNode = new Entities.Node();
            elementNode.Id = node.id;
            elementNode.Coordinate = new GeoCoordinate(latitude, longitude);

            if (node.keys.Any())
            {
                elementNode.Tags = new Dictionary<string, string>(node.keys.Count);
                for (int tagIdx = 0; tagIdx < node.keys.Count; tagIdx++)
                {
                    var keyBytes = block.stringtable.s[(int) node.keys[tagIdx]];
                    string key = String.Intern(Encoding.UTF8.GetString(keyBytes, 0, keyBytes.Length));
                    var valueBytes = block.stringtable.s[(int) node.vals[tagIdx]];
                    string value = String.Intern(Encoding.UTF8.GetString(valueBytes, 0, valueBytes.Length));
                    elementNode.Tags.Add(key, value);
                }
            }
            
            if (!Elements.ContainsKey(elementNode.Id))
                Elements.Add(elementNode.Id, elementNode);
            else
            {
                // merge tags if element is present in collection
                if (elementNode.Tags != null && elementNode.Tags.Count > 0)
                {
                    var tags = Elements[elementNode.Id].Tags;
                    foreach (var keyValuePair in elementNode.Tags)
                        if (!tags.ContainsKey(keyValuePair.Key))
                            tags.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            if (_unresolvedNodes.Contains(elementNode.Id))
            {
                _unresolvedNodes.Remove(elementNode.Id);
                elementNode.IsOutOfBox = true;
            }
            _nodeIds.Add(elementNode.Id);
        }

        private static readonly ObjectListPool<long> LongListPool = new ObjectListPool<long>(2, 64);
        private void SearchWay(PrimitiveBlock block, Formats.Pbf.Way way)
        {
            long nodeId = 0;
            var nodeIds = LongListPool.New();
            var notFound = true;
            var refCount = way.refs.Count;
            for (int nodeIdx = 0; nodeIdx < refCount; nodeIdx++)
            {
                nodeId = nodeId + way.refs[nodeIdx];
                nodeIds.Add(nodeId);
                if (notFound)
                    notFound = !_nodeIds.Contains(nodeId);
            }

            // Way is out of bbox
            if (notFound)
            {
                LongListPool.Store(nodeIds);
                return;
            }

            var elementWay = new Entities.Way {Id = way.id, NodeIds = nodeIds};
            // Push all unresolved node ids to scan later
            // Unresolved node situation happends when we have cross-zone ways
            // We want to display them fully
            foreach (var nId in elementWay.NodeIds)
            {
                if (!_nodeIds.Contains(nId) && !_unresolvedNodes.Contains(nId))
                    _unresolvedNodes.Add(nId);
            }

            if (way.keys.Any())
            {
                var keyCount = way.keys.Count;
                elementWay.Tags = new Dictionary<string, string>(keyCount);
                for (int tagIdx = 0; tagIdx < keyCount; tagIdx++)
                {
                    var keyBytes = block.stringtable.s[(int) way.keys[tagIdx]];
                    string key = String.Intern(Encoding.UTF8.GetString(keyBytes, 0, keyBytes.Length));
                    var valueBytes = block.stringtable.s[(int) way.vals[tagIdx]];
                    string value = String.Intern(Encoding.UTF8.GetString(valueBytes, 0, valueBytes.Length));
                    elementWay.Tags.Add(key, value);
                }
            }

            // TODO this situation occurs rarely; need to investigate
            if (!Elements.ContainsKey(elementWay.Id))
                Elements.Add(elementWay.Id, elementWay);

            _wayIds.Add(elementWay.Id);
        }

        private void SearchRelation(PrimitiveBlock block, Formats.Pbf.Relation relation)
        {
            var elementRelation = new Entities.Relation();
            elementRelation.Id = relation.id;
            if (relation.types.Count > 0)
            {
                elementRelation.Members = new List<RelationMember>();
                long memberId = 0;
                for (int memberIdx = 0; memberIdx < relation.types.Count; memberIdx++)
                {
                    memberId = memberId + relation.memids[memberIdx];

                    if (_nodeIds.Contains(memberId) || _wayIds.Contains(memberId) || _relationIds.Contains(memberId))
                    {
                        var roleBytes = block.stringtable.s[relation.roles_sid[memberIdx]];
                        string role = String.Intern(Encoding.UTF8.GetString(roleBytes, 0, roleBytes.Length));
                        var member = new RelationMember();
                        member.MemberId = memberId;
                        member.Role = role;
                        switch (relation.types[memberIdx])
                        {
                            case Formats.Pbf.Relation.MemberType.NODE:
                                member.Member = new Entities.Node();
                                break;
                            case Formats.Pbf.Relation.MemberType.WAY:
                                member.Member = new Entities.Way();
                                break;
                            case Formats.Pbf.Relation.MemberType.RELATION:
                                member.Member = new Entities.Relation();
                                break;
                        }

                        elementRelation.Members.Add(member);
                    }
                }
                if (!elementRelation.Members.Any())
                    return;
            }
            if (relation.keys.Count > 0)
            {
                elementRelation.Tags = new Dictionary<string, string>(relation.keys.Count);
                for (int tagIdx = 0; tagIdx < relation.keys.Count; tagIdx++)
                {
                    var keyBytes = block.stringtable.s[(int) relation.keys[tagIdx]];
                    string key = String.Intern(Encoding.UTF8.GetString(keyBytes, 0, keyBytes.Length));
                    var valueBytes = block.stringtable.s[(int) relation.vals[tagIdx]];
                    string value = String.Intern(Encoding.UTF8.GetString(valueBytes, 0, valueBytes.Length));
                    elementRelation.Tags.Add(key, value);
                }
            }
            // TODO this situation occurs rarely; need to investigate
            if (!Elements.ContainsKey(elementRelation.Id))
                Elements.Add(elementRelation.Id, elementRelation);
            _relationIds.Add(elementRelation.Id);
        }

        /// <summary>
        ///     Calculates block header offset for l
        /// </summary>
        private static double CalculateBboxOffset(double latLon, double latLonBlockOffset, PrimitiveBlock block)
        {
            return (latLon/.000000001 - latLonBlockOffset)/block.granularity;
        }

        private void ProcessPrimitiveBlock(PrimitiveBlock block, OffsetBoundingBox obbox)
        {
            if (block.primitivegroup != null)
            {
                foreach (PrimitiveGroup primitivegroup in block.primitivegroup)
                {
                    if (primitivegroup.dense != null)
                    {
                        int keyValsIdx = 0;
                        long currentId = 0;
                        long currentLat = 0;
                        long currentLon = 0;

                        var count = primitivegroup.dense.id.Count;
                        var nodes = new List<Formats.Pbf.Node>();
                        for (int idx = 0; idx < count; idx++)
                        {
                            // do the delta decoding stuff.
                            currentId = currentId + primitivegroup.dense.id[idx];
                            currentLat = currentLat + primitivegroup.dense.lat[idx];
                            currentLon = currentLon + primitivegroup.dense.lon[idx];

                            bool shouldAdd = !((obbox == null && !_unresolvedNodes.Contains(currentId)) ||
                                               (obbox != null && !obbox.Contains(currentLat, currentLon)));
                                

                            var node = new Formats.Pbf.Node();
                            node.id = currentId;
                            node.lat = currentLat;
                            node.lon = currentLon;

                            // get the keys/vals.
                            List<int> keysVals = primitivegroup.dense.keys_vals;
                            var keysValsCount = keysVals.Count;
                            if (shouldAdd) node.Initialize();
                            while (keysValsCount > keyValsIdx && keysVals[keyValsIdx] != 0)
                            {
                                if(shouldAdd)
                                    node.keys.Add((uint) keysVals[keyValsIdx]);
                                keyValsIdx++;
                                if(shouldAdd)
                                    node.vals.Add((uint) keysVals[keyValsIdx]);
                                keyValsIdx++;
                            }
                            keyValsIdx++;
                            if(shouldAdd)
                                nodes.Add(node);
                        }
                        primitivegroup.nodes = nodes;
                    }
                }
            }
        }

        /// <summary>
        ///     Converts geocoordinates of bbox to block-specific offsets
        /// </summary>
        private class OffsetBoundingBox
        {
            private readonly double _minLat;
            private readonly double _minLong;
            private readonly double _maxLat;
            private readonly double _maxLong;

            public OffsetBoundingBox(BoundingBox bbox, PrimitiveBlock block)
            {
                _minLat = CalculateBboxOffset(bbox.MinPoint.Latitude, block.lat_offset, block);
                _minLong = CalculateBboxOffset(bbox.MinPoint.Longitude, block.lon_offset, block);
                _maxLat = CalculateBboxOffset(bbox.MaxPoint.Latitude, block.lat_offset, block);
                _maxLong = CalculateBboxOffset(bbox.MaxPoint.Longitude, block.lon_offset, block);
            }

            public bool Contains(double latitude, double longitude)
            {
                return (_maxLat > latitude && latitude >= _minLat) &&
                       (_maxLong > longitude && longitude >= _minLong);
            }
        }

        #endregion

        /// <inheritdoc />
        public Entities.Node GetNode(long id)
        {
            return GetElement<Entities.Node>(id);
        }

        /// <inheritdoc />
        public Entities.Way GetWay(long id)
        {
            return GetElement<Entities.Way>(id);
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            Elements.Clear();
        }

        /// <inheritdoc />
        public Entities.Relation GetRelation(long id)
        {
            return GetElement<Entities.Relation>(id);
        }

        private T GetElement<T>(long id) where T : Entities.Element
        {
            if (!Elements.ContainsKey(id))
                return null;
            return Elements[id] as T;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reader.Dispose();
                _stream.Dispose();
            }
        }
    }
}