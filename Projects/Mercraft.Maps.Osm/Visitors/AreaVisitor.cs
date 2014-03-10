
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Extensions;
using Way = Mercraft.Maps.Osm.Entities.Way;
using Node = Mercraft.Maps.Osm.Entities.Node;

namespace Mercraft.Maps.Osm.Visitors
{
    public class AreaVisitor: IElementVisitor
    {
        private readonly IScene _scene;

        [Dependency]
        public AreaVisitor(IScene scene)
        {
            _scene = scene;
        }

        public void VisitNode(Node node)
        {
            
        }

        public void VisitWay(Way way)
        {
            if ( !way.IsComplete ||  !IsArea(way.Tags))
                return;

            var area = new Area()
            {
                Id = way.Id.ToString(),
                Points = way.GetPoints(),

                // TODO remove tags on building
                Tags = way.Tags
                    .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                    .ToList()
            };

            _scene.AddArea(area);
        }

        public void VisitRelation(Relation relation)
        {
            
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
