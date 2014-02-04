
using System;
using System.Collections.Generic;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections;
using Mercraft.Maps.Core.Collections.Tags;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Relation class.
    /// </summary>
    public class CompleteRelation : CompleteOsmGeo
    {
        /// <summary>
        /// Holds the members of this relation.
        /// </summary>
        private readonly IList<CompleteRelationMember> _members;

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        internal protected CompleteRelation(long id)
            : base(id)
        {
            _members = new List<CompleteRelationMember>();
        }

        /// <summary>
        /// Creates a new relation using a string table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stringTable"></param>
        internal protected CompleteRelation(ObjectTable<string> stringTable, long id)
            : base(stringTable, id)
        {
            _members = new List<CompleteRelationMember>();
        }

        /// <summary>
        /// Returns the relation type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.Relation; }
        }

        /// <summary>
        /// Gets the relation members.
        /// </summary>
        public IList<CompleteRelationMember> Members
        {
            get
            {
                return _members;
            }
        }

        /// <summary>
        /// Find a member in this relation with the given role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public CompleteOsmBase FindMember(string role)
        {
            if (this.Members != null)
            {
                foreach (CompleteRelationMember member in this.Members)
                {
                    if (member.Role == role)
                    {
                        return member.Member;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Converts this relation into it's simple counterpart.
        /// </summary>
        /// <returns></returns>
        public override Element ToSimple()
        {
            var relation = new Relation();
            relation.Id = this.Id;
            relation.ChangeSetId = this.ChangeSetId;
            relation.Tags = this.Tags;
            relation.TimeStamp = this.TimeStamp;
            relation.UserId = this.UserId;
            relation.UserName = this.User;
            relation.Version = (ulong?)this.Version;
            relation.Visible = this.Visible;

            relation.Members = new List<RelationMember>();
            foreach (CompleteRelationMember member in this.Members)
            {
                var simple_member = new RelationMember();
                simple_member.MemberId = member.Member.Id;
                simple_member.MemberRole = member.Role;
                switch (member.Member.Type)
                {
                    case CompleteOsmType.Node:
                        simple_member.MemberType = ElementType.Node;
                        break;
                    case CompleteOsmType.Relation:
                        simple_member.MemberType = ElementType.Relation;
                        break;
                    case CompleteOsmType.Way:
                        simple_member.MemberType = ElementType.Way;
                        break;
                }
                relation.Members.Add(simple_member);
            }
            return relation;
        }

        /// <summary>
        /// Returns all the coordinates in this way in the same order as the nodes.
        /// </summary>
        /// <returns></returns>
        public IList<GeoCoordinate> GetCoordinates()
        {
            var coordinates = new List<GeoCoordinate>();

            for (int idx = 0; idx < this.Members.Count; idx++)
            {
                if (this.Members[idx].Member is CompleteNode)
                {
                    var node = this.Members[idx].Member as CompleteNode;
                    coordinates.Add(node.Coordinate);
                }
                else if (this.Members[idx].Member is CompleteWay)
                {
                    var way = this.Members[idx].Member as CompleteWay;
                    coordinates.AddRange(way.GetCoordinates());
                }
                else if (this.Members[idx].Member is CompleteRelation)
                {
                    var relation = this.Members[idx].Member as CompleteRelation;
                    coordinates.AddRange(relation.GetCoordinates());
                }
            }

            return coordinates;
        }

        #region Relation factory functions

        /// <summary>
        /// Creates a relation with a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteRelation Create(long id)
        {
            return new CompleteRelation(id);
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        /// <param name="nodes"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(Relation simpleRelation,
            IDictionary<long, CompleteNode> nodes,
            IDictionary<long, CompleteWay> ways,
            IDictionary<long, CompleteRelation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (ways == null) throw new ArgumentNullException("ways");
            if (relations == null) throw new ArgumentNullException("relations");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            CompleteRelation relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (Tag pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case ElementType.Node:
                        CompleteNode node = null;
                        if (nodes.TryGetValue(memberId, out node))
                        {
                            member.Member = node;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Way:
                        CompleteWay way = null;
                        if (ways.TryGetValue(memberId, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Relation:
                        CompleteRelation relationMember = null;
                        if (relations.TryGetValue(memberId, out relationMember))
                        {
                            member.Member = relationMember;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

            return relation;
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        /// <param name="osmGeoSource"></param>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(Relation simpleRelation, IOsmGeoSource osmGeoSource)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            CompleteRelation relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            if (simpleRelation.Tags != null)
            {
                foreach (Tag pair in simpleRelation.Tags)
                {
                    relation.Tags.Add(pair);
                }
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case ElementType.Node:
                        Node simpleNode = osmGeoSource.GetNode(memberId);
                        if(simpleNode == null)
                        {
                            return null;
                        }
                        CompleteNode completeNode = CompleteNode.CreateFrom(simpleNode);
                        if (completeNode != null)
                        {
                            member.Member = completeNode;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Way:
                        Way simpleWay = osmGeoSource.GetWay(memberId);
                        if(simpleWay == null)
                        {
                            return null;
                        }
                        CompleteWay completeWay = CompleteWay.CreateFrom(simpleWay, osmGeoSource);
                        if (completeWay != null)
                        {
                            member.Member = completeWay;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Relation:
                        Relation simpleRelationMember = osmGeoSource.GetRelation(memberId);
                        if(simpleRelationMember == null)
                        {
                            return null;
                        }
                        CompleteRelation completeRelation = CompleteRelation.CreateFrom(simpleRelationMember, osmGeoSource);
                        if (completeRelation != null)
                        {
                            member.Member = completeRelation;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

            return relation;
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        /// <param name="osmGeoSource"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(Relation simpleRelation, IOsmGeoSource osmGeoSource,
            IDictionary<long, CompleteWay> ways,
            IDictionary<long, CompleteRelation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            CompleteRelation relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (Tag pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case ElementType.Node:
                        Node simpleNode = osmGeoSource.GetNode(memberId);
                        if (simpleNode != null)
                        {
                            member.Member = CompleteNode.CreateFrom(simpleNode);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Way:
                        CompleteWay completeWay;
                        if (!ways.TryGetValue(memberId, out completeWay))
                        {
                            Way simpleWay = osmGeoSource.GetWay(memberId);
                            if (simpleWay != null)
                            {
                                completeWay = CompleteWay.CreateFrom(simpleWay, osmGeoSource);
                            }
                        }
                        if (completeWay != null)
                        {
                            member.Member = completeWay;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Relation:
                        CompleteRelation completeRelation;
                        if (!relations.TryGetValue(memberId, out completeRelation))
                        {
                            Relation simpleRelationMember = osmGeoSource.GetRelation(memberId);
                            if (simpleRelationMember != null)
                            {
                                completeRelation = CompleteRelation.CreateFrom(simpleRelationMember, osmGeoSource);
                            }
                        }
                        if (completeRelation != null)
                        {
                            member.Member = completeRelation;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteRelation Create(ObjectTable<string> table, long id)
        {
            return new CompleteRelation(table, id);
        }

        /// <summary>
        /// Creates a new relation from a SimpleRelation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simpleRelation"></param>
        /// <param name="nodes"></param>
        /// <param name="ways"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(ObjectTable<string> table, Relation simpleRelation,
            IDictionary<long, CompleteNode> nodes,
            IDictionary<long, CompleteWay> ways,
            IDictionary<long, CompleteRelation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (ways == null) throw new ArgumentNullException("ways");
            if (relations == null) throw new ArgumentNullException("relations");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            CompleteRelation relation = Create(table, simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (Tag pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case ElementType.Node:
                        CompleteNode node = null;
                        if (nodes.TryGetValue(memberId, out node))
                        {
                            member.Member = node;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Way:
                        CompleteWay way = null;
                        if (ways.TryGetValue(memberId, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case ElementType.Relation:
                        CompleteRelation relationMember = null;
                        if (relations.TryGetValue(memberId, out relationMember))
                        {
                            member.Member = relationMember;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;

            return relation;
        }

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteChangeSet CreateChangeSet(long id)
        {
            return new CompleteChangeSet(id);
        }

        #endregion

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("http://www.openstreetmap.org/?relation={0}",
                this.Id);
        }
    }
}
