
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Extensions;
using Way = Mercraft.Maps.Osm.Entities.Way;

namespace Mercraft.Maps.Osm.Visitors
{
    public class WayVisitor: ElementVisitor
    {
        public WayVisitor(IScene scene) : base(scene)
        {
        }

        public override void VisitWay(Way way)
        {
            if ( !way.IsPolygon)
                return;

            if (!IsArea(way.Tags))
            {
                MergeTags(way);
                Scene.AddWay(new Core.Scene.Models.Way()
                {
                    Id = way.Id,
                    Points = way.GetPoints(),
                    Tags = way.Tags
                    .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                    .ToList()
                });

                return;
            }

            MergeTags(way);
            var area = new Area()
            {
                Id = way.Id,
                Points = way.GetPoints(),

                Tags = way.Tags
                    .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                    .ToList()
            };

            Scene.AddArea(area);
        }

        private void MergeTags(Way way)
        {
            foreach (var node in way.Nodes)
            {
                foreach (var tag in node.Tags)
                {
                    if (IsMergeTag(tag) && way.Tags.All(t => t.Key != tag.Key))
                    {
                        way.Tags.Add(new Tag(tag.Key, tag.Value));
                    }
                }
            }
        }

        private bool IsMergeTag(Tag tag)
        {
            return tag.Key.StartsWith("addr:");
        }

        private bool IsArea(ICollection<Tag> tags)
        {
            return ((tags.ContainsKey("building") && !tags.IsFalse("building")) ||
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
                    (tags.ContainsKey("aeroway") && !tags.IsFalse("aeroway")));
        }
    }
}
