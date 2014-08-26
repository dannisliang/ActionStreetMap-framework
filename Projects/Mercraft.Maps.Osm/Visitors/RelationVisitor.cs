using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Utilities;
using Mercraft.Maps.Osm.Entities;
using Way = Mercraft.Maps.Osm.Entities.Way;

namespace Mercraft.Maps.Osm.Visitors
{
    public class RelationVisitor : ElementVisitor
    {
        private readonly IScene _scene;
        public RelationVisitor(IScene scene) : base(scene)
        {
            _scene = scene;
        }

        public override void VisitRelation(Relation relation)
        {
            // see http://wiki.openstreetmap.org/wiki/Relation:multipolygon

            if (relation.Tags.ContainsKeyValue("type", "multipolygon"))
            {
                var innerWays = new List<Way>();
                var outerWays = new List<Way>();
                foreach (var member in relation.Members)
                {
                    var way = member.Member as Way;
                    if(way == null)
                        continue;

                    if (member.Role == "inner")
                        innerWays.Add(way);
                    else if(member.Role == "outer")
                        outerWays.Add(way);
                }

                if (!outerWays.Any())
                    return;
                
                var points = new List<GeoCoordinate>();
                foreach (var outerWay in outerWays)
                {
                    points.AddRange(outerWay.GetPoints());
                }              

                // TODO process inner points!

                var area = new Area
                {
                    Id = relation.Id,
                    Points = points.ToArray(),
                    Tags = relation.Tags
                };
                _scene.AddArea(area);
            }
        }
    }
}
