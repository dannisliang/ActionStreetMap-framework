
using System;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections;
using Mercraft.Maps.Core.Collections.Tags;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Node class.
    /// </summary>
    public class CompleteNode : CompleteOsmGeo, IEquatable<CompleteNode>
    {
        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="id"></param>
        internal protected CompleteNode(long id)
            : base(id)
        {

        }

        /// <summary>
        /// Creates a new node using a string table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stringTable"></param>
        internal protected CompleteNode(ObjectTable<string> stringTable, long id)
            : base(stringTable, id)
        {

        }

        /// <summary>
        /// Returns the node type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.Node; }
        }

        /// <summary>
        /// The coordinates of this node.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }

        /// <summary>
        /// Converts this node to it's simple counterpart.
        /// </summary>
        /// <returns></returns>
        public override Element ToSimple()
        {
            Node simple_node = new Node();
            simple_node.Id = this.Id;
            simple_node.ChangeSetId = this.ChangeSetId;
            simple_node.Latitude = this.Coordinate.Latitude;
            simple_node.Longitude = this.Coordinate.Longitude;
            simple_node.Tags = this.Tags;
            simple_node.TimeStamp = this.TimeStamp;
            simple_node.UserId = this.UserId;
            simple_node.UserName = this.User;
            simple_node.Version = (ulong?)this.Version;
            simple_node.Visible = this.Visible;
            return simple_node;
        }

        #region IEquatable<CompleteNode> Members

        /// <summary>
        /// Returns true if the given object equals the other in content.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CompleteNode other)
        {
            if (other != null)
            {
                return other.Id == this.Id;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Returns a description of this node.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Coordinate != null)
            {
                return String.Format("http://www.openstreetmap.org/?node={0}:[{1};{2}]",
                    this.Id,
                    this.Coordinate.Longitude,
                    this.Coordinate.Latitude);
            }
            return String.Format("http://www.openstreetmap.org/?node={0}",
                this.Id);
        }

        /// <summary>
        /// Copies all properties of this node onto the given node without the id.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CopyTo(CompleteNode n)
        {
            foreach (Tag tag in this.Tags)
            {
                n.Tags.Add(tag.Key, tag.Value);
            }

            n.TimeStamp = this.TimeStamp;
            n.User = this.User;
            n.UserId = this.UserId;
            n.Version = this.Version;
            n.Visible = this.Visible;
        }

        /// <summary>
        /// Returns an exact copy of this way.
        /// 
        /// WARNING: even the id is copied!
        /// </summary>
        /// <returns></returns>
        public CompleteNode Copy()
        {
            CompleteNode n = new CompleteNode(this.Id);
            this.CopyTo(n);
            return n;
        }

        #region Node factory functions

        /// <summary>
        /// Creates a new node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteNode Create(long id)
        {
            return new CompleteNode(id);
        }

        /// <summary>
        /// Creates a new node using a given string table with the given id.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteNode Create(ObjectTable<string> table, long id)
        {
            return new CompleteNode(table, id);
        }

        /// <summary>
        /// Creates a new node from a SimpleNode.
        /// </summary>
        /// <param name="simpleNode"></param>
        /// <returns></returns>
        public static CompleteNode CreateFrom(Node simpleNode)
        {
            if (simpleNode == null) throw new ArgumentNullException("simpleNode");
            if (simpleNode.Id == null) throw new Exception("simpleNode.id is null");
            if (simpleNode.Latitude == null) throw new Exception("simpleNode.Latitude is null");
            if (simpleNode.Longitude == null) throw new Exception("simpleNode.Longitude is null");

            CompleteNode node = CompleteNode.Create(simpleNode.Id.Value);

            node.ChangeSetId = simpleNode.ChangeSetId;
            node.Coordinate = new GeoCoordinate(simpleNode.Latitude.Value, simpleNode.Longitude.Value);
            if (simpleNode.Tags != null)
            {
                foreach (Tag pair in simpleNode.Tags)
                {
                    node.Tags.Add(pair);
                }
            }
            node.TimeStamp = simpleNode.TimeStamp;
            node.User = simpleNode.UserName;
            node.UserId = simpleNode.UserId;
            node.Version = simpleNode.Version.HasValue ? (long)simpleNode.Version.Value : (long?)null;
            node.Visible = simpleNode.Visible.HasValue && simpleNode.Visible.Value;

            return node;
        }

        /// <summary>
        /// Creates a new node from a SimpleNode using a string table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simpleNode"></param>
        /// <returns></returns>
        public static CompleteNode CreateFrom(ObjectTable<string> table, Node simpleNode)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (simpleNode == null) throw new ArgumentNullException("simpleNode");
            if (simpleNode.Id == null) throw new Exception("simpleNode.id is null");
            if (simpleNode.Latitude == null) throw new Exception("simpleNode.Latitude is null");
            if (simpleNode.Longitude == null) throw new Exception("simpleNode.Longitude is null");

            CompleteNode node = CompleteNode.Create(table, simpleNode.Id.Value);

            node.ChangeSetId = simpleNode.ChangeSetId;
            node.Coordinate = new GeoCoordinate(simpleNode.Latitude.Value, simpleNode.Longitude.Value);
            if (simpleNode.Tags != null)
            {
                foreach (Tag pair in simpleNode.Tags)
                {
                    node.Tags.Add(pair);
                }
            }
            node.TimeStamp = simpleNode.TimeStamp;
            node.User = simpleNode.UserName;
            node.UserId = simpleNode.UserId;
            node.Version = simpleNode.Version.HasValue ? (long)simpleNode.Version.Value : (long?)null;
            node.Visible = simpleNode.Visible.HasValue && simpleNode.Visible.Value;

            return node;
        }

        #endregion
    }
}