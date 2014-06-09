using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Formats.Pbf;

namespace Mercraft.Maps.Osm.Data
{
    public class PbfElementSource : IElementSource
    {
        /// <summary>
        ///     Holds the Pbf reader.
        /// </summary>
        private PbfReader _reader;

        /// <summary>
        ///     Holds the source of the data.
        /// </summary>
        private Stream _stream;

        private HashSet<long> _nodeIds;
        private HashSet<long> _wayIds;
        private HashSet<long> _relationIds;

        private HashSet<long> _unresolvedNodes;

        protected Dictionary<long, Element> Elements;

        protected PbfElementSource()
        {
            Elements = new Dictionary<long, Element>();
        }

        /// <summary>
        ///     Creates a new source of Pbf formated OSM data.
        /// </summary>
        public PbfElementSource(Stream stream): this()
        {
            SetStream(stream);
        }

        protected void SetStream(Stream stream)
        {
            _stream = stream;
            _reader = new PbfReader(_stream);
            ResetPrivateState();
        }

        public virtual IEnumerable<Element> Get(BoundingBox bbox)
        {
            FillElements(bbox);
            ResetPrivateState();
            return Elements.Values;
        }

        private void ResetPrivateState()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            _nodeIds = new HashSet<long>();
            _wayIds = new HashSet<long>();
            _relationIds = new HashSet<long>();
            _unresolvedNodes = new HashSet<long>();
        }

        #region Fill elements collection from pbf stream logic

        /// <summary>
        ///     Fills Elements collection with elements located in bounding box, but with undersolved references
        /// </summary>
        private void FillElements(BoundingBox bbox)
        {
            PrimitiveBlock block = _reader.MoveNext();
            while (block != null)
            {
                var obbox = new OffsetBoundingBox(bbox, block);
                ProcessPrimitiveBlock(block, obbox);

                foreach (var primitiveGroup in block.primitivegroup)
                {
                    foreach (var node in primitiveGroup.nodes)
                    {
                        // check bbox
                        //if (obbox.Contains(node.lat, node.lon))
                        SearchNode(block, node);
                    }

                    foreach (var way in primitiveGroup.ways)
                    {
                        SearchWay(block, way);
                    }

                    foreach (var relation in primitiveGroup.relations)
                    {
                        SearchRelation(block, relation);
                    }
                }
                block = _reader.MoveNext();
            }

            // Resolve unresolved nodes
            // NOTE this code increases time consumption almost in two times
            // due to IO/parsing/unzipping staff. We can't cache PrimitiveBlock
            // cause it leads to increasing memory consumption
            // TODO need to thing about how to improve it keeping corresponding logic
            _stream.Seek(0, SeekOrigin.Begin);

            block = _reader.MoveNext();
            while (block != null)
            {
                ProcessPrimitiveBlock(block, null);
                foreach (var primitiveGroup in block.primitivegroup)
                {
                    foreach (var node in primitiveGroup.nodes)
                    {
                        SearchNode(block, node);
                    }
                }
                block = _reader.MoveNext();
            }
        }

        private void SearchNode(PrimitiveBlock block, Formats.Pbf.Node node)
        {
            var latitude = .000000001*(block.lat_offset + (block.granularity*(double) node.lat));
            var longitude = .000000001*(block.lon_offset + (block.granularity*(double) node.lon));

            var elementNode = new Entities.Node();
            elementNode.Id = node.id;
            elementNode.Latitude = latitude;
            elementNode.Longitude = longitude;

            if (node.keys.Any())
            {
                elementNode.Tags = new List<KeyValuePair<string, string>>();
                for (int tagIdx = 0; tagIdx < node.keys.Count; tagIdx++)
                {
                    string key = String.Intern(Encoding.UTF8.GetString(block.stringtable.s[(int) node.keys[tagIdx]]));
                    string value = String.Intern(Encoding.UTF8.GetString(block.stringtable.s[(int) node.vals[tagIdx]]));
                    elementNode.Tags.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            // TODO this situation occurs rarely; need to investigate
            // is it proper way to ignore (or should we merge nodes?)
            if (!Elements.ContainsKey(elementNode.Id))
                Elements.Add(elementNode.Id, elementNode);
            if (_unresolvedNodes.Contains(elementNode.Id))
                _unresolvedNodes.Remove(elementNode.Id);
            _nodeIds.Add(elementNode.Id);
        }

        private void SearchWay(PrimitiveBlock block, Formats.Pbf.Way way)
        {
            long nodeId = 0;
            var nodeIds = new List<long>(way.refs.Count);
            for (int nodeIdx = 0; nodeIdx < way.refs.Count; nodeIdx++)
            {
                nodeId = nodeId + way.refs[nodeIdx];
                nodeIds.Add(nodeId);
            }

            // Way is out of bbox
            if (!nodeIds.Any(nid => _nodeIds.Contains(nid)))
                return;

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
                elementWay.Tags = new List<KeyValuePair<string, string>>();
                for (int tagIdx = 0; tagIdx < way.keys.Count; tagIdx++)
                {
                    string key = String.Intern(Encoding.UTF8.GetString(block.stringtable.s[(int) way.keys[tagIdx]]));
                    string value = String.Intern(Encoding.UTF8.GetString(block.stringtable.s[(int) way.vals[tagIdx]]));
                    elementWay.Tags.Add(new KeyValuePair<string, string>(key, value));
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
                        string role =
                            String.Intern(Encoding.UTF8.GetString(block.stringtable.s[relation.roles_sid[memberIdx]]));
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
                elementRelation.Tags = new List<KeyValuePair<string, string>>();
                for (int tagIdx = 0; tagIdx < relation.keys.Count; tagIdx++)
                {
                    string key = String.Intern(Encoding.UTF8.GetString(block.stringtable.s[(int) relation.keys[tagIdx]]));
                    string value =
                        String.Intern(Encoding.UTF8.GetString(block.stringtable.s[(int) relation.vals[tagIdx]]));
                    elementRelation.Tags.Add(new KeyValuePair<string, string>(key, value));
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

                        var nodes = new List<Formats.Pbf.Node>();
                        var count = primitivegroup.dense.id.Count;
                        for (int idx = 0; idx < count; idx++)
                        {
                            // do the delta decoding stuff.
                            currentId = currentId + primitivegroup.dense.id[idx];
                            currentLat = currentLat + primitivegroup.dense.lat[idx];
                            currentLon = currentLon + primitivegroup.dense.lon[idx];

                            if (obbox == null && !_unresolvedNodes.Contains(currentId))
                                continue;

                            if (obbox != null && !obbox.Contains(currentLat, currentLon))
                                continue;

                            var node = new Formats.Pbf.Node();
                            node.id = currentId;
                            node.lat = currentLat;
                            node.lon = currentLon;

                            // get the keys/vals.
                            List<int> keysVals = primitivegroup.dense.keys_vals;
                            var keysValsCount = keysVals.Count;
                            while (keysValsCount > keyValsIdx && keysVals[keyValsIdx] != 0)
                            {
                                node.keys.Add((uint) keysVals[keyValsIdx]);
                                keyValsIdx++;
                                node.vals.Add((uint) keysVals[keyValsIdx]);
                                keyValsIdx++;
                            }
                            keyValsIdx++;
                            nodes.Add(node);
                        }
                        primitivegroup.nodes = nodes;
                    }
                }
            }
        }

        //  in order to avoid unnecessary calculations for every node
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

        public Entities.Node GetNode(long id)
        {
            return GetElement<Entities.Node>(id);
        }

        public Entities.Way GetWay(long id)
        {
            return GetElement<Entities.Way>(id);
        }

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
    }
}