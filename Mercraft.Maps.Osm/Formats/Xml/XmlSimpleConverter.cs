// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Formats.Xml
{
    /// <summary>
    /// Converts simple objects from/to xml equivalents.
    /// </summary>
    internal static class XmlSimpleConverter
    {

        internal static Node ConvertToSimple(OsmSharp.Osm.Xml.v0_6.node nd)
        {
            Node node = new Node();

            // set id
            if (nd.idSpecified)
            {
                node.Id = nd.id;
            }

            // set changeset.
            if (nd.changesetSpecified)
            {
                node.ChangeSetId = nd.changeset;
            }

            // set visible.
            if (nd.visibleSpecified)
            {
                node.Visible = nd.visible;
            }
            else
            { // if visible is not specified it is default true.
                node.Visible = true;
            }

            // set timestamp.
            if (nd.timestampSpecified)
            {
                node.TimeStamp = nd.timestamp;
            }

            // set latitude.
            if (nd.latSpecified)
            {
                node.Latitude = nd.lat;
            }

            // set longitude.
            if (nd.lonSpecified)
            {
                node.Longitude = nd.lon;
            }

            // set uid
            if (nd.uidSpecified)
            {
                node.UserId = nd.uid;
            }

            // set version
            if (nd.versionSpecified)
            {
                node.Version = nd.version;
            }

            // set user
            node.UserName = nd.user;

            // set tags.
            node.Tags = XmlSimpleConverter.ConvertToTags(nd.tag);

            return node;
        }

        internal static Way ConvertToSimple(OsmSharp.Osm.Xml.v0_6.way wa)
        {
            Way way = new Way();

            // set id
            if (wa.idSpecified)
            {
                way.Id = wa.id;
            }

            // set changeset.
            if (wa.changesetSpecified)
            {
                way.ChangeSetId = wa.changeset;
            }

            // set visible.
            if (wa.visibleSpecified)
            {
                way.Visible = wa.visible;
            }
            else
            { // if visible is not specified it is default true.
                way.Visible = true;
            }

            // set timestamp.
            if (wa.timestampSpecified)
            {
                way.TimeStamp = wa.timestamp;
            }

            // set uid
            if (wa.uidSpecified)
            {
                way.UserId = wa.uid;
            }

            // set version
            if (wa.versionSpecified)
            {
                way.Version = wa.version;
            }

            // set user
            way.UserName = wa.user;

            // set tags.
            way.Tags = XmlSimpleConverter.ConvertToTags(wa.tag);

            // set nodes.
            if (wa.nd != null && wa.nd.Length > 0)
            {
                way.NodeIds = new List<long>();
                for (int idx = 0; idx < wa.nd.Length; idx++)
                {
                    way.NodeIds.Add(wa.nd[idx].@ref);
                }
            }

            return way;
        }

        internal static Relation ConvertToSimple(OsmSharp.Osm.Xml.v0_6.relation re)
        {
            Relation relation = new Relation();

            // set id
            if (re.idSpecified)
            {
                relation.Id = re.id;
            }

            // set changeset.
            if (re.changesetSpecified)
            {
                relation.ChangeSetId = re.changeset;
            }

            // set visible.
            if (re.visibleSpecified)
            {
                relation.Visible = re.visible;
            }
            else
            { // if visible is not specified it is default true.
                relation.Visible = true;
            }

            // set timestamp.
            if (re.timestampSpecified)
            {
                relation.TimeStamp = re.timestamp;
            }

            // set uid
            if (re.uidSpecified)
            {
                relation.UserId = re.uid;
            }

            // set version
            if (re.versionSpecified)
            {
                relation.Version = re.version;
            }

            // set user
            relation.UserName = re.user;

            // set tags.
            relation.Tags = XmlSimpleConverter.ConvertToTags(re.tag);

            // set members.
            if (re.member != null && re.member.Length > 0)
            {
                relation.Members = new List<RelationMember>();
                for (int idx = 0; idx < re.member.Length; idx++)
                {
                    OsmSharp.Osm.Xml.v0_6.member mem = re.member[idx];
                    RelationMember relation_member = new RelationMember();
                    // set memberid
                    if (mem.refSpecified)
                    {
                        relation_member.MemberId = mem.@ref;
                    }

                    // set role.
                    relation_member.MemberRole = mem.role;

                    // set type.
                    if (mem.typeSpecified)
                    {
                        switch (mem.type)
                        {
                            case OsmSharp.Osm.Xml.v0_6.memberType.node:
                                relation_member.Member = new Node();
                                break;
                            case OsmSharp.Osm.Xml.v0_6.memberType.way:
                                relation_member.Member = new Way();
                                break;
                            case OsmSharp.Osm.Xml.v0_6.memberType.relation:
                                relation_member.Member = new Way();
                                break;
                        }
                    }

                    relation.Members.Add(relation_member);
                }
            }

            return relation;
        }

        private static ICollection<Tag> ConvertToTags(OsmSharp.Osm.Xml.v0_6.tag[] tag)
        {
            List<Tag> tags = null;
            if (tag != null && tag.Length > 0)
            {
                tags = new List<Tag>();
                foreach (OsmSharp.Osm.Xml.v0_6.tag t in tag)
                {
                    tags.Add( new Tag(t.k, t.v));
                }
            }
            return tags;
        }
    }
}
