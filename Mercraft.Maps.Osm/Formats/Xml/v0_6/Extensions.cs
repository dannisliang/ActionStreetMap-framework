
using System;
using System.Collections.Generic;
using System.Globalization;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Format.Xml.v0_6;
using Mercraft.Models;

namespace Mercraft.Maps.Osm.Formats.Xml.v0_6
{
    /// <summary>
    /// Some common extensions.
    /// </summary>
    public static class Extensions
    {
        #region Xml <-> OsmSharp.Osm Conversions

        #region Convert From Xml

        /// <summary>
        /// Extensions method for a bound.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static GeoCoordinateBox ConvertFrom(this bounds bounds)
        {
            if (bounds != null)
            {
                return new GeoCoordinateBox(
                    new GeoCoordinate((double)bounds.maxlat, (double)bounds.maxlon),
                    new GeoCoordinate((double)bounds.minlat, (double)bounds.minlon));
            }
            return null;
        }

        /// <summary>
        /// Extensions method for a bound.
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        public static GeoCoordinateBox ConvertFrom(this bound bound)
        {
            if (bound != null)
            {
                string[] bounds = bound.box.Split(',');
                return new GeoCoordinateBox(
                    new GeoCoordinate(double.Parse(bounds[0], CultureInfo.InvariantCulture),
                        double.Parse(bounds[1], CultureInfo.InvariantCulture)),
                    new GeoCoordinate(double.Parse(bounds[2], CultureInfo.InvariantCulture),
                        double.Parse(bounds[3], CultureInfo.InvariantCulture)));
            }
            return null;
        }

        /// <summary>
        /// Converts an Xml relation to an Osm domain model relation.
        /// </summary>
        /// <param name="xml_obj"></param>
        /// <returns></returns>
        public static Relation ConvertFrom(this relation xml_obj)
        {
            // create a new node and immidiately set the id.
            Relation new_obj = new Relation();
            new_obj.Id = xml_obj.id;

            // set the members
            new_obj.Members = new List<RelationMember>(xml_obj.member.Length);
            for (int idx = 0; idx < xml_obj.member.Length; idx++)
            {
                member member = xml_obj.member[idx];
                RelationMember simpleMember = new RelationMember();
                simpleMember.MemberId = member.@ref;
                simpleMember.MemberRole = member.role;
                if (member.refSpecified && member.typeSpecified)
                {
                    switch (member.type)
                    {
                        case memberType.node:
                            simpleMember.Member = new Entities.Node();
                            break;
                        case memberType.relation:
                            simpleMember.Member = new Entities.Relation();
                            break;
                        case memberType.way:
                            simpleMember.Member = new Entities.Way();
                            break;
                    }
                }
                else
                { // way cannot be converted; member could not be created!
                    return null;
                }
                new_obj.Members.Add(simpleMember);
            }

            // set the tags.
            if (xml_obj.tag != null)
            {
                foreach (Mercraft.Maps.Osm.Format.Xml.v0_6.tag tag in xml_obj.tag)
                {
                    new_obj.Tags.Add(new Tag(tag.k, tag.v));
                }
            }

            // set the user info.
            if (xml_obj.uidSpecified)
            {
                new_obj.UserId = xml_obj.uid;
            }
            new_obj.UserName = xml_obj.user;

            // set the changeset info.
            if (xml_obj.changesetSpecified)
            {
                new_obj.ChangeSetId = xml_obj.changeset;
            }

            // set the timestamp flag.
            if (xml_obj.timestampSpecified)
            {
                new_obj.TimeStamp = xml_obj.timestamp;
            }

            // set the visible flag.
            new_obj.Visible = xml_obj.visible;
            return new_obj;
        }

        /// <summary>
        /// Converts an Xml way to an Osm domain model way.
        /// </summary>
        /// <param name="xml_obj"></param>
        /// <returns></returns>
        public static Way ConvertFrom(this way xml_obj)
        {
            // create a new node and immidiately set the id.
            Way new_obj = new Way();
            new_obj.Id = xml_obj.id;

            // set the nodes.
            new_obj.NodeIds = new List<long>(xml_obj.nd.Length);
            for (int idx = 0; idx < xml_obj.nd.Length;idx++)
            {
                nd node = xml_obj.nd[idx];
                if (node.refSpecified)
                {
                    new_obj.NodeIds.Add(node.@ref);
                }
                else
                { // way cannot be converted; node was not found!
                    return null;
                }
            }

            // set the tags.
            if (xml_obj.tag != null)
            {
                foreach (Mercraft.Maps.Osm.Format.Xml.v0_6.tag tag in xml_obj.tag)
                {
                    new_obj.Tags.Add(new Tag(tag.k, tag.v));
                }
            }

            // set the user info.
            if (xml_obj.uidSpecified)
            {
                new_obj.UserId = xml_obj.uid;
            }
            new_obj.UserName = xml_obj.user;

            // set the changeset info.
            if (xml_obj.changesetSpecified)
            {
                new_obj.ChangeSetId = xml_obj.changeset;
            }

            // set the timestamp flag.
            if (xml_obj.timestampSpecified)
            {
                new_obj.TimeStamp = xml_obj.timestamp;
            }

            // set the visible flag.
            new_obj.Visible = xml_obj.visible;
            return new_obj;
        }

        /// <summary>
        /// Converts an Xml node to an Osm domain model node.
        /// </summary>
        /// <param name="xml_obj"></param>
        /// <returns></returns>
        public static Node ConvertFrom(this node xml_obj)
        {
            // create a new node an immidiately set the id.
            Node new_obj = new Node();
            new_obj.Id = xml_obj.id;

            // set the long- and latitude
            if (!xml_obj.latSpecified || !xml_obj.lonSpecified)
            {
                throw new ArgumentNullException("Latitude and/or longitude cannot be null!");
            }
            new_obj.Latitude = (double)xml_obj.lat;
            new_obj.Longitude = (double)xml_obj.lon;

            // set the tags.
            if (xml_obj.tag != null)
            {
                foreach (Mercraft.Maps.Osm.Format.Xml.v0_6.tag tag in xml_obj.tag)
                {
                    new_obj.Tags.Add(new Tag(tag.k, tag.v));
                }
            }

            // set the user info.
            if (xml_obj.uidSpecified)
            {
                new_obj.UserId = (int)xml_obj.uid;
            }
            new_obj.UserName = xml_obj.user;

            // set the changeset info.
            if (xml_obj.changesetSpecified)
            {
                new_obj.ChangeSetId = (int)xml_obj.changeset;
            }

            // set the timestamp flag.
            if (xml_obj.timestampSpecified)
            {
                new_obj.TimeStamp = xml_obj.timestamp;
            }

            // set the visible flag.
            new_obj.Visible = xml_obj.visible;
            return new_obj;
        }

        #endregion

        #region Convert To Xml

        /// <summary>
        /// Extensions method for a geocoordinatebox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bounds ConvertTo(this GeoCoordinateBox box)
        {
            bounds b = new bounds();

            b.maxlat = box.MaxLat;
            b.maxlatSpecified = true;
            b.maxlon = box.MaxLon;
            b.maxlonSpecified = true;
            b.minlat = box.MinLat;
            b.minlatSpecified = true;
            b.minlon = box.MinLon;
            b.minlonSpecified = true;

            return b;
        }

        ///// <summary>
        ///// Converts an domain model changeset to and Xml changeset.
        ///// </summary>
        ///// <param name="changeset"></param>
        ///// <returns></returns>
        //public static changeset ConvertTo(this ChangeSet changeset)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Converts a domain model relation to an Xml relation.
        ///// </summary>
        ///// <param name="dom_obj"></param>
        ///// <returns></returns>
        //public static relation ConvertTo(this SimpleRelation dom_obj)
        //{
        //    relation xml_obj = new relation();

        //    // set the changeset.
        //    if (dom_obj.ChangeSetId.HasValue)
        //    {
        //        xml_obj.changeset = dom_obj.ChangeSetId.Value;
        //        xml_obj.changesetSpecified = true;
        //    }

        //    // set the id.
        //    if (dom_obj.Id.HasValue)
        //    {
        //        xml_obj.id = dom_obj.Id.Value;
        //        xml_obj.idSpecified = true;
        //    }
        //    else
        //    {
        //        xml_obj.idSpecified = false;
        //    }

        //    if (dom_obj.Tags != null)
        //    {
        //        xml_obj.tag = new tag[dom_obj.Tags.Count];
        //        int idx = 0;
        //        foreach (var tag in dom_obj.Tags)
        //        {
        //            tag t = new tag();
        //            t.k = tag.Key;
        //            t.v = tag.Value;
        //            xml_obj.tag[idx] = t;
        //            idx++;
        //        }
        //    }

        //    // set the timestamp.
        //    if (dom_obj.TimeStamp.HasValue)
        //    {
        //        xml_obj.timestamp = dom_obj.TimeStamp.Value;
        //        xml_obj.timestampSpecified = true;
        //    }

        //    // set the user data.
        //    if (dom_obj.UserId.HasValue)
        //    {
        //        xml_obj.uid = dom_obj.UserId.Value;
        //        xml_obj.uidSpecified = true;
        //    }
        //    xml_obj.user = xml_obj.user;

        //    // set the version.
        //    if (dom_obj.Version.HasValue)
        //    {
        //        xml_obj.version = (ulong)dom_obj.Version.Value;
        //        xml_obj.versionSpecified = true;
        //    }

        //    // set the visible.
        //    if (dom_obj.Visible.HasValue)
        //    {
        //        xml_obj.visible = dom_obj.Visible.Value;
        //        xml_obj.visibleSpecified = true;
        //    }
        //    else
        //    {
        //        xml_obj.visibleSpecified = false;
        //    }

        //    // set the way-specific properties.
        //    xml_obj.member = new member[dom_obj.Members.Count];
        //    for (int idx = 0; idx < dom_obj.Members.Count; idx++)
        //    {
        //        RelationMember dom_member = dom_obj.Members[idx];
        //        member m = new member();
                
        //        switch(dom_member.Member.Type)
        //        {
        //            case OsmType.Node:
        //                m.type = memberType.node;
        //                m.typeSpecified = true;
        //                break;
        //            case OsmType.Relation:
        //                m.type = memberType.relation;
        //                m.typeSpecified = true;
        //                break;
        //            case OsmType.Way:
        //                m.type = memberType.way;
        //                m.typeSpecified = true;
        //                break;
        //        }

        //        m.@ref = dom_member.Member.Id;
        //        m.refSpecified = true;
        //        m.role = dom_member.Role;

        //        xml_obj.member[idx] = m;
        //    }

        //    return xml_obj;
        //}

        ///// <summary>
        ///// Converts a domain model way to an Xml way.
        ///// </summary>
        ///// <param name="dom_obj"></param>
        ///// <returns></returns>
        //public static way ConvertTo(this Way dom_obj)
        //{
        //    way xml_obj = new way();

        //    // set the changeset.
        //    if (dom_obj.ChangeSetId.HasValue)
        //    {
        //        xml_obj.changeset = dom_obj.ChangeSetId.Value;
        //        xml_obj.changesetSpecified = true;
        //    }

        //    // set the id.
        //    xml_obj.id = dom_obj.Id;
        //    xml_obj.idSpecified = true;

        //    if (dom_obj.Tags != null)
        //    {
        //        xml_obj.tag = new tag[dom_obj.Tags.Count];
        //        int idx = 0;
        //        foreach (var tag in dom_obj.Tags)
        //        {
        //            tag t = new tag();
        //            t.k = tag.Key;
        //            t.v = tag.Value;
        //            xml_obj.tag[idx] = t;
        //            idx++;
        //        }
        //    }

        //    // set the timestamp.
        //    if (dom_obj.TimeStamp.HasValue)
        //    {
        //        xml_obj.timestamp = dom_obj.TimeStamp.Value;
        //        xml_obj.timestampSpecified = true;
        //    }

        //    // set the user data.
        //    if (dom_obj.UserId.HasValue)
        //    {
        //        xml_obj.uid = dom_obj.UserId.Value;
        //        xml_obj.uidSpecified = true;
        //    }
        //    xml_obj.user = xml_obj.user;

        //    // set the version.
        //    if (dom_obj.Version.HasValue)
        //    {
        //        xml_obj.version = (ulong)dom_obj.Version.Value;
        //        xml_obj.versionSpecified = true;
        //    }

        //    // set the visible.
        //    xml_obj.visible = dom_obj.Visible;
        //    xml_obj.visibleSpecified = true;

        //    // set the way-specific properties.
        //    xml_obj.nd = new nd[dom_obj.Nodes.Count];
        //    for(int idx = 0;idx < dom_obj.Nodes.Count;idx++)
        //    {
        //        nd n = new nd();
        //        n.@ref =  dom_obj.Nodes[idx].Id;
        //        n.refSpecified = true;
        //        xml_obj.nd[idx] = n;
        //    }

        //    return xml_obj;
        //}

        ///// <summary>
        ///// Converts an Xml node to an Osm domain model node.
        ///// </summary>
        ///// <param name="dom_obj"></param>
        ///// <returns></returns>
        //public static node ConvertTo(this Node dom_obj)
        //{
        //    node xml_obj = new node();
            
        //    // set the changeset.
        //    if(dom_obj.ChangeSetId.HasValue)
        //    {
        //        xml_obj.changeset = dom_obj.ChangeSetId.Value;
        //        xml_obj.changesetSpecified = true;
        //    }

        //    // set the id.
        //    xml_obj.id = dom_obj.Id;
        //    xml_obj.idSpecified = true;

        //    if (dom_obj.Tags != null)
        //    {
        //        xml_obj.tag = new tag[dom_obj.Tags.Count];
        //        int idx = 0;
        //        foreach (var tag in dom_obj.Tags)
        //        {
        //            tag t = new tag();
        //            t.k = tag.Key;
        //            t.v = tag.Value;
        //            xml_obj.tag[idx] = t;
        //            idx++;
        //        }
        //    }

        //    // set the timestamp.
        //    if (dom_obj.TimeStamp.HasValue)
        //    {
        //        xml_obj.timestamp = dom_obj.TimeStamp.Value;
        //        xml_obj.timestampSpecified = true;
        //    }

        //    // set the user data.
        //    if (dom_obj.UserId.HasValue)
        //    {
        //        xml_obj.uid = dom_obj.UserId.Value;
        //        xml_obj.uidSpecified = true;
        //    }
        //    xml_obj.user = xml_obj.user;

        //    // set the version.
        //    if (dom_obj.Version.HasValue)
        //    {
        //        xml_obj.version = (ulong)dom_obj.Version.Value;
        //        xml_obj.versionSpecified = true;
        //    }

        //    // set the visible.
        //    xml_obj.visible = dom_obj.Visible;
        //    xml_obj.visibleSpecified = true;

        //    // set the node-specific properties.
        //    xml_obj.lat = dom_obj.Coordinate.Latitude;
        //    xml_obj.latSpecified = true;
        //    xml_obj.lon = dom_obj.Coordinate.Longitude;
        //    xml_obj.lonSpecified = true;

        //    return xml_obj;
        //}

        /// <summary>
        /// Converts an Xml node to an Osm domain model node.
        /// </summary>
        /// <param name="dom_obj"></param>
        /// <returns></returns>
       /* public static node ConvertTo(this Entities.Node dom_obj)
        {
            node xml_obj = new node();

            // set the changeset.
            if (dom_obj.ChangeSetId.HasValue)
            {
                xml_obj.changeset = dom_obj.ChangeSetId.Value;
                xml_obj.changesetSpecified = true;
            }

            // set the id.
            if (dom_obj.Id.HasValue)
            {
                xml_obj.id = dom_obj.Id.Value;
                xml_obj.idSpecified = true;
            }

            if (dom_obj.Tags != null)
            {
                xml_obj.tag = new tag[dom_obj.Tags.Count];
                int idx = 0;
                foreach (var tag in dom_obj.Tags)
                {
                    tag t = new tag();
                    t.k = tag.Key;
                    t.v = tag.Value;
                    xml_obj.tag[idx] = t;
                    idx++;
                }
            }

            // set the timestamp.
            if (dom_obj.TimeStamp.HasValue)
            {
                xml_obj.timestamp = dom_obj.TimeStamp.Value;
                xml_obj.timestampSpecified = true;
            }

            // set the user data.
            if (dom_obj.UserId.HasValue)
            {
                xml_obj.uid = dom_obj.UserId.Value;
                xml_obj.uidSpecified = true;
            }
            xml_obj.user = xml_obj.user;

            // set the version.
            if (dom_obj.Version.HasValue)
            {
                xml_obj.version = (ulong)dom_obj.Version.Value;
                xml_obj.versionSpecified = true;
            }

            // set the visible.
            xml_obj.visible = dom_obj.Visible.Value;
            xml_obj.visibleSpecified = true;

            // set the node-specific properties.
            xml_obj.lat = dom_obj.Latitude.Value;
            xml_obj.latSpecified = true;
            xml_obj.lon = dom_obj.Longitude.Value;
            xml_obj.lonSpecified = true;

            return xml_obj;
        }

        /// <summary>
        /// Converts a domain model way to an Xml way.
        /// </summary>
        /// <param name="dom_obj"></param>
        /// <returns></returns>
        public static way ConvertTo(this Entities.Way dom_obj)
        {
            way xml_obj = new way();

            // set the changeset.
            if (dom_obj.ChangeSetId.HasValue)
            {
                xml_obj.changeset = dom_obj.ChangeSetId.Value;
                xml_obj.changesetSpecified = true;
            }

            // set the id.
            if (dom_obj.Id.HasValue)
            {
                xml_obj.id = dom_obj.Id.Value;
                xml_obj.idSpecified = true;
            }
            else
            {
                xml_obj.idSpecified = false;
            }

            if (dom_obj.Tags != null)
            {
                xml_obj.tag = new tag[dom_obj.Tags.Count];
                int idx = 0;
                foreach (var tag in dom_obj.Tags)
                {
                    tag t = new tag();
                    t.k = tag.Key;
                    t.v = tag.Value;
                    xml_obj.tag[idx] = t;
                    idx++;
                }
            }

            // set the timestamp.
            if (dom_obj.TimeStamp.HasValue)
            {
                xml_obj.timestamp = dom_obj.TimeStamp.Value;
                xml_obj.timestampSpecified = true;
            }

            // set the user data.
            if (dom_obj.UserId.HasValue)
            {
                xml_obj.uid = dom_obj.UserId.Value;
                xml_obj.uidSpecified = true;
            }
            xml_obj.user = xml_obj.user;

            // set the version.
            if (dom_obj.Version.HasValue)
            {
                xml_obj.version = (ulong)dom_obj.Version.Value;
                xml_obj.versionSpecified = true;
            }

            // set the visible.
            if (dom_obj.Visible.HasValue)
            {
                xml_obj.visible = dom_obj.Visible.Value;
                xml_obj.visibleSpecified = true;
            }
            else
            {
                xml_obj.visibleSpecified = false;
            }

            // set the way-specific properties.
            xml_obj.nd = new nd[dom_obj.Nodes.Count];
            for (int idx = 0; idx < dom_obj.Nodes.Count; idx++)
            {
                nd n = new nd();
                n.@ref = dom_obj.Nodes[idx];
                n.refSpecified = true;
                xml_obj.nd[idx] = n;
            }

            return xml_obj;
        }

        /// <summary>
        /// Converts a domain model relation to an Xml relation.
        /// </summary>
        /// <param name="dom_obj"></param>
        /// <returns></returns>
        public static relation ConvertTo(this Entities.Relation dom_obj)
        {
            relation xml_obj = new relation();

            // set the changeset.
            if (dom_obj.ChangeSetId.HasValue)
            {
                xml_obj.changeset = dom_obj.ChangeSetId.Value;
                xml_obj.changesetSpecified = true;
            }

            // set the id.
            if (dom_obj.Id.HasValue)
            {
                xml_obj.id = dom_obj.Id.Value;
                xml_obj.idSpecified = true;
            }
            else
            {
                xml_obj.idSpecified = false;
            }

            if (dom_obj.Tags != null)
            {
                xml_obj.tag = new tag[dom_obj.Tags.Count];
                int idx = 0;
                foreach (var tag in dom_obj.Tags)
                {
                    tag t = new tag();
                    t.k = tag.Key;
                    t.v = tag.Value;
                    xml_obj.tag[idx] = t;
                    idx++;
                }
            }

            // set the timestamp.
            if (dom_obj.TimeStamp.HasValue)
            {
                xml_obj.timestamp = dom_obj.TimeStamp.Value;
                xml_obj.timestampSpecified = true;
            }

            // set the user data.
            if (dom_obj.UserId.HasValue)
            {
                xml_obj.uid = dom_obj.UserId.Value;
                xml_obj.uidSpecified = true;
            }
            xml_obj.user = xml_obj.user;

            // set the version.
            if (dom_obj.Version.HasValue)
            {
                xml_obj.version = (ulong)dom_obj.Version.Value;
                xml_obj.versionSpecified = true;
            }

            // set the visible.
            if (dom_obj.Visible.HasValue)
            {
                xml_obj.visible = dom_obj.Visible.Value;
                xml_obj.visibleSpecified = true;
            }
            else
            {
                xml_obj.visibleSpecified = false;
            }

            // set the way-specific properties.
            xml_obj.member = new member[dom_obj.Members.Count];
            for (int idx = 0; idx < dom_obj.Members.Count; idx++)
            {
                RelationMember dom_member = dom_obj.Members[idx];
                member m = new member();

                if (dom_member.Member != null)
                {
                    if (dom_member.Member is Node)
                    {
                        m.type = memberType.node;
                        m.typeSpecified = true;
                    }
                    else if (dom_member.Member is Way)
                    {
                        m.type = memberType.relation;
                        m.typeSpecified = true;
                    }
                    else
                    {
                        m.type = memberType.way;
                        m.typeSpecified = true;
                    }
                }
                else
                {
                    m.typeSpecified = false;
                }

                if (dom_member.MemberId.HasValue)
                {
                    m.@ref = dom_member.MemberId.Value;
                    m.refSpecified = true;
                }
                else
                {
                    m.refSpecified = false;
                }
                m.role = dom_member.MemberRole;

                xml_obj.member[idx] = m;
            }

            return xml_obj;
        }*/

        #endregion

        #endregion
    }
}
