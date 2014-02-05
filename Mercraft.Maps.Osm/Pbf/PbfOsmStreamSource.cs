// Mercraft.Maps - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections.Tags;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Streams;

namespace Mercraft.Maps.Osm.Pbf
{
    /// <summary>
    /// A source of Pbf formatted OSM data.
    /// </summary>
    public class PbfOsmStreamSource : OsmStreamSource, IPbfOsmPrimitiveConsumer
    {
        /// <summary>
        /// Holds the source of the data.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Creates a new source of Pbf formated OSM data.
        /// </summary>
        /// <param name="stream"></param>
        public PbfOsmStreamSource(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes the current source.
        /// </summary>
        public override void Initialize()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            this.InitializePbfReader();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            KeyValuePair<PrimitiveBlock, object> nextPbfPrimitive = 
                this.MoveToNextPrimitive();

            if (nextPbfPrimitive.Value != null)
            { // there is a primitive.
                _current = this.Convert(nextPbfPrimitive);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private Element _current;

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        /// <returns></returns>
        public override Element Current()
        {
            return _current;
        }

        /// <summary>
        /// Resetting this data source 
        /// </summary>
        public override void Reset()
        {
            _current = null;
            _stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        #region Primitive Conversion

        /// <summary>
        /// Converts simple primitives.
        /// </summary>
        /// <param name="PbfPrimitive"></param>
        /// <returns></returns>
        internal Element Convert(KeyValuePair<PrimitiveBlock, object> PbfPrimitive)
        {
            if (PbfPrimitive.Value == null || PbfPrimitive.Key == null)
            {
                throw new ArgumentNullException("PbfPrimitive");
            }

            PrimitiveBlock block = PbfPrimitive.Key; // get the block properties this object comes from.
            if (PbfPrimitive.Value is Mercraft.Maps.Osm.Pbf.Node)
            {
                var node = (PbfPrimitive.Value as Mercraft.Maps.Osm.Pbf.Node);
                var simpleNode = new Entities.Node();
                simpleNode.ChangeSetId = node.info.changeset;
                simpleNode.Id = node.id;
                simpleNode.Latitude = .000000001 * ((double)block.lat_offset 
                    + ((double)block.granularity * (double)node.lat));
                simpleNode.Longitude = .000000001 * ((double)block.lon_offset
                    + ((double)block.granularity * (double)node.lon));
                if (node.keys.Count > 0)
                {
                    simpleNode.Tags = new TagsCollection();
                    for (int tag_idx = 0; tag_idx < node.keys.Count; tag_idx++)
                    {
                        string key = Encoding.UTF8.GetString(block.stringtable.s[(int)node.keys[tag_idx]]);
                        string value = Encoding.UTF8.GetString(block.stringtable.s[(int)node.vals[tag_idx]]);

                        if (!simpleNode.Tags.ContainsKey(key))
                        {
                            simpleNode.Tags.Add(new Tag() { Key = key, Value = value });
                        }
                    }
                }
                simpleNode.TimeStamp = FromUnixTime((long)node.info.timestamp * 
                    (long)block.date_granularity);
                simpleNode.Visible = true;
                simpleNode.Version = (uint)node.info.version;
                simpleNode.UserId = node.info.uid;
                simpleNode.UserName = Encoding.UTF8.GetString(block.stringtable.s[node.info.user_sid]);
                simpleNode.Version = (ulong)node.info.version;
                simpleNode.Visible = true;

                return simpleNode;
            }
            else if (PbfPrimitive.Value is Mercraft.Maps.Osm.Pbf.Way)
            {
                var way = (PbfPrimitive.Value as Mercraft.Maps.Osm.Pbf.Way);

                var simple_way = new Entities.Way();
                simple_way.Id = way.id;
                simple_way.NodeIds = new List<long>(way.refs.Count);
                long node_id = 0;
                for (int node_idx = 0; node_idx < way.refs.Count; node_idx++)
                {
                    node_id = node_id + way.refs[node_idx];
                    simple_way.NodeIds.Add(node_id);
                }
                if (way.keys.Count > 0)
                {
                    simple_way.Tags = new TagsCollection();
                    for (int tag_idx = 0; tag_idx < way.keys.Count; tag_idx++)
                    {
                        string key = Encoding.UTF8.GetString(block.stringtable.s[(int)way.keys[tag_idx]]);
                        string value = Encoding.UTF8.GetString(block.stringtable.s[(int)way.vals[tag_idx]]);

                        if (!simple_way.Tags.ContainsKey(key))
                        {
                            simple_way.Tags.Add(new Tag(key, value));
                        }
                    }
                }
                if (way.info != null)
                { // add the metadata if any.
                    simple_way.ChangeSetId = way.info.changeset;
                    simple_way.TimeStamp = FromUnixTime((long)way.info.timestamp *
                        (long)block.date_granularity);
                    simple_way.UserId = way.info.uid;
                    simple_way.UserName = Encoding.UTF8.GetString(block.stringtable.s[way.info.user_sid]);
                    simple_way.Version = (ulong)way.info.version;
                }
                simple_way.Visible = true;

                return simple_way;
            }
            else if (PbfPrimitive.Value is Mercraft.Maps.Osm.Pbf.Relation)
            {
                var relation = (PbfPrimitive.Value as Mercraft.Maps.Osm.Pbf.Relation);

                var simple_relation = new Entities.Relation();
                simple_relation.Id = relation.id;
                if (relation.types.Count > 0)
                {
                    simple_relation.Members = new List<RelationMember>();
                    long member_id = 0;
                    for (int member_idx = 0; member_idx < relation.types.Count; member_idx++)
                    {
                        member_id = member_id + relation.memids[member_idx];
                        string role = Encoding.UTF8.GetString(
                            block.stringtable.s[relation.roles_sid[member_idx]]);
                        var member = new RelationMember();
                        member.MemberId = member_id;
                        member.MemberRole = role;
                        switch (relation.types[member_idx])
                        {
                            case Relation.MemberType.NODE:
                                member.MemberType = ElementType.Node;
                                break;
                            case Relation.MemberType.WAY:
                                member.MemberType = ElementType.Way;
                                break;
                            case Relation.MemberType.RELATION:
                                member.MemberType = ElementType.Relation;
                                break;
                        }

                        simple_relation.Members.Add(member);
                    }
                }
                if (relation.keys.Count > 0)
                {
                    simple_relation.Tags = new TagsCollection();
                    for (int tag_idx = 0; tag_idx < relation.keys.Count; tag_idx++)
                    {
                        string key = Encoding.UTF8.GetString(block.stringtable.s[(int)relation.keys[tag_idx]]);
                        string value = Encoding.UTF8.GetString(block.stringtable.s[(int)relation.vals[tag_idx]]);

                        if (!simple_relation.Tags.ContainsKey(key))
                        {
                            simple_relation.Tags.Add(new Tag(key, value));
                        }
                    }
                }
                if (relation.info != null)
                { // read metadata if any.
                    simple_relation.ChangeSetId = relation.info.changeset;
                    simple_relation.TimeStamp = FromUnixTime((long)relation.info.timestamp *
                        (long)block.date_granularity);
                    simple_relation.UserId = relation.info.uid;
                    simple_relation.UserName = Encoding.UTF8.GetString(block.stringtable.s[relation.info.user_sid]);
                    simple_relation.Version = (ulong)relation.info.version;
                }
                simple_relation.Visible = true;

                return simple_relation;
            }
            throw new Exception(string.Format("Pbf primitive with type {0} not supported!",
                PbfPrimitive.GetType().ToString()));
        }


        private static DateTime FromUnixTime(long milliseconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(milliseconds);
        }

        #endregion

        #region Pbf Blocks Reader

        /// <summary>
        /// Holds the Pbf reader.
        /// </summary>
        private PbfReader _reader;

        /// <summary>
        /// Holds the primitives decompressor.
        /// </summary>
        private Mercraft.Maps.Osm.Pbf.Decompressor _decompressor;

        /// <summary>
        /// Initializes the Pbf reader.
        /// </summary>
        private void InitializePbfReader()
        {
            _reader = new PbfReader(_stream);
            _decompressor = new Mercraft.Maps.Osm.Pbf.Decompressor(this);

            this.InitializeBlockCache();
        }

        /// <summary>
        /// Moves the Pbf reader to the next primitive or returns one of the cached ones.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> MoveToNextPrimitive()
        {
            KeyValuePair<PrimitiveBlock, object> next = this.DeQueuePrimitive();
            if (next.Value == null)
            {
                PrimitiveBlock block = _reader.MoveNext();
                if (block != null)
                {
                    _decompressor.ProcessPrimitiveBlock(block);
                    next = this.DeQueuePrimitive();
                }
            }
            return next;
        }

        #region Block Cache

        /// <summary>
        /// Holds the cached primitives.
        /// </summary>
        private Queue<KeyValuePair<PrimitiveBlock, object>> _cachedPrimitives;

        /// <summary>
        /// Initializes the block cache.
        /// </summary>
        private void InitializeBlockCache()
        {
            _cachedPrimitives = new Queue<KeyValuePair<PrimitiveBlock, object>>();
        }

        /// <summary>
        /// Queues the primitives.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="primitive"></param>
        private void QueuePrimitive(PrimitiveBlock block, object primitive)
        {
            _cachedPrimitives.Enqueue(new KeyValuePair<PrimitiveBlock, object>(block, primitive));
        }

        /// <summary>
        /// DeQueues a primitive.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> DeQueuePrimitive()
        {
            if (_cachedPrimitives.Count > 0)
            {
                return _cachedPrimitives.Dequeue();
            }
            return new KeyValuePair<PrimitiveBlock, object>();
        }

        #endregion

        #endregion

        /// <summary>
        /// Processes a node.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="node"></param>
        void IPbfOsmPrimitiveConsumer.ProcessNode(PrimitiveBlock block, Node node)
        {
            this.QueuePrimitive(block, node);
        }

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        void IPbfOsmPrimitiveConsumer.ProcessWay(PrimitiveBlock block, Way way)
        {
            this.QueuePrimitive(block, way);
        }

        /// <summary>
        /// Processes a relation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        void IPbfOsmPrimitiveConsumer.ProcessRelation(PrimitiveBlock block, Relation relation)
        {
            this.QueuePrimitive(block, relation);
        }
    }
}