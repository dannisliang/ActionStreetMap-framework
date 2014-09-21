using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Utilities;
using Mercraft.Maps.Osm.Extensions;
using Way = Mercraft.Maps.Osm.Entities.Way;

namespace Mercraft.Maps.Osm.Visitors
{
    public class WayVisitor : ElementVisitor
    {
        public WayVisitor(IScene scene) : base(scene)
        {
        }

        public override void VisitWay(Way way)
        {
            if (!IsArea(way.Tags))
            {
                Scene.AddWay(new Core.Scene.Models.Way
                {
                    Id = way.Id,
                    Points = way.GetPoints(),
                    Tags = GetMergedTags(way)
                });

                return;
            }

            if (!way.IsPolygon)
                return;

            var area = new Area
            {
                Id = way.Id,
                Points = way.GetPoints(),
                Tags = GetMergedTags(way)
            };
            Scene.AddArea(area);
        }

        /// <summary>
        /// Returns merged tags. We cannot do this in place as Way can be reused
        /// in case of cross tile processing logic is applied
        /// </summary>
        private List<KeyValuePair<string,string>> GetMergedTags(Way way)
        {
            var tags = way.Tags == null ? 
                new List<KeyValuePair<string, string>>() : 
                new List<KeyValuePair<string, string>>(way.Tags);
            foreach (var node in way.Nodes)
            {
                if (node.Tags == null)
                    continue;
                foreach (var tag in node.Tags)
                {
                    if (IsMergeTag(tag) && tags.All(t => t.Key != tag.Key))
                    {
                        tags.Add(new KeyValuePair<string, string>(tag.Key, tag.Value));
                    }
                }
            }
            return tags;
        }

        private bool IsMergeTag(KeyValuePair<string, string> tag)
        {
            return tag.Key.StartsWith("addr:");
        }

        private bool IsArea(IList<KeyValuePair<string, string>> tags)
        {
            return (tags != null) &&
                   ((tags.ContainsKey("building") && !tags.IsFalse("building")) ||
                   (tags.ContainsKey("building:part") && !tags.IsFalse("building:part")) ||
                    (tags.ContainsKey("landuse") && !tags.IsFalse("landuse")) ||
                    (tags.ContainsKey("amenity") && !tags.IsFalse("amenity")) ||
                    (tags.ContainsKey("harbour") && !tags.IsFalse("harbour")) ||
                    (tags.ContainsKey("historic") && !tags.IsFalse("historic")) ||
                    (tags.ContainsKey("leisure") && !tags.IsFalse("leisure")) ||
                    (tags.ContainsKey("man_made") && !tags.IsFalse("man_made")) ||
                    (tags.ContainsKey("military") && !tags.IsFalse("military")) ||
                    (tags.ContainsKey("natural") && !tags.IsFalse("natural")) ||
                    (tags.ContainsKey("office") && !tags.IsFalse("office")) ||
                    (tags.ContainsKey("place") && !tags.IsFalse("place")) ||
                    (tags.ContainsKey("power") && !tags.IsFalse("power")) ||
                    (tags.ContainsKey("public_transport") && !tags.IsFalse("public_transport")) ||
                    (tags.ContainsKey("shop") && !tags.IsFalse("shop")) ||
                    (tags.ContainsKey("sport") && !tags.IsFalse("sport")) ||
                    (tags.ContainsKey("tourism") && !tags.IsFalse("tourism")) ||
                    (tags.ContainsKey("waterway") && !tags.IsFalse("waterway")) ||
                    (tags.ContainsKey("wetland") && !tags.IsFalse("wetland")) ||
                    (tags.ContainsKey("water") && !tags.IsFalse("water")) ||
                    (tags.ContainsKey("aeroway") && !tags.IsFalse("aeroway")) ||
                    (tags.ContainsKey("addr:housenumber") && !tags.IsFalse("addr:housenumber")) ||
                    (tags.ContainsKey("addr:housename") && !tags.IsFalse("addr:housename"))
                    );
        }
    }
}