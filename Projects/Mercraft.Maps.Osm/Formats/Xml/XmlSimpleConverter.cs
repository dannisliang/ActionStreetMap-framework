
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Formats.Xml
{
    /// <summary>
    /// Converts simple objects from/to xml equivalents.
    /// </summary>
    internal static class XmlSimpleConverter
    {

        internal static Node ConvertToSimple(Mercraft.Maps.Osm.Format.Xml.v0_6.node nd)
        {
            Node node = new Node();

            // set id
            if (nd.idSpecified)
            {
                node.Id = nd.id;
            }

            // set latitude.
            float lat = 0, lon = 0;
            if (nd.latSpecified)
            {
                lat = (float)nd.lat;
            }

            // set longitude.
            if (nd.lonSpecified)
            {
                lon = (float)nd.lon;
            }
            node.Coordinate = new GeoCoordinate(lat, lon);

          /*  // set uid
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
            node.UserName = nd.user;*/

            // set tags.
            node.Tags = XmlSimpleConverter.ConvertToTags(nd.tag);

            return node;
        }

        internal static Way ConvertToSimple(Mercraft.Maps.Osm.Format.Xml.v0_6.way wa)
        {
            Way way = new Way();

            // set id
            if (wa.idSpecified)
            {
                way.Id = wa.id;
            }

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

        internal static Relation ConvertToSimple(Mercraft.Maps.Osm.Format.Xml.v0_6.relation re)
        {
            Relation relation = new Relation();

            // set id
            if (re.idSpecified)
            {
                relation.Id = re.id;
            }

            // set tags.
            relation.Tags = XmlSimpleConverter.ConvertToTags(re.tag);

            // set members.
            if (re.member != null && re.member.Length > 0)
            {
                relation.Members = new List<RelationMember>();
                for (int idx = 0; idx < re.member.Length; idx++)
                {
                    Mercraft.Maps.Osm.Format.Xml.v0_6.member mem = re.member[idx];
                    RelationMember relation_member = new RelationMember();
                    // set memberid
                    if (mem.refSpecified)
                    {
                        relation_member.MemberId = mem.@ref;
                    }

                    // set role.
                    relation_member.Role = mem.role;

                    // set type.
                    if (mem.typeSpecified)
                    {
                        switch (mem.type)
                        {
                            case Mercraft.Maps.Osm.Format.Xml.v0_6.memberType.node:
                                relation_member.Member = new Node();
                                break;
                            case Mercraft.Maps.Osm.Format.Xml.v0_6.memberType.way:
                                relation_member.Member = new Way();
                                break;
                            case Mercraft.Maps.Osm.Format.Xml.v0_6.memberType.relation:
                                relation_member.Member = new Way();
                                break;
                        }
                    }

                    relation.Members.Add(relation_member);
                }
            }

            return relation;
        }

        private static Dictionary<string, string> ConvertToTags(Mercraft.Maps.Osm.Format.Xml.v0_6.tag[] tag)
        {
            Dictionary<string, string> tags = null;
            if (tag != null && tag.Length > 0)
            {
                tags = new Dictionary<string, string>(tag.Length);
                foreach (Mercraft.Maps.Osm.Format.Xml.v0_6.tag t in tag)
                {
                    tags.Add(t.k, t.v);
                }
            }
            return tags;
        }
    }
}
