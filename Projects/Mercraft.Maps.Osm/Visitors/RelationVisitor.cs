using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Maps.Osm.Entities;
using Way = Mercraft.Maps.Osm.Entities.Way;

namespace Mercraft.Maps.Osm.Visitors
{
    public class RelationVisitor : ElementVisitor
    {
        public RelationVisitor(IModelVisitor modelVisitor, IObjectPool objectPool)
            : base(modelVisitor, objectPool)
        {
        }

        public override void VisitRelation(Relation relation)
        {
            // see http://wiki.openstreetmap.org/wiki/Relation:multipolygon
            string actualValue;
            if (relation.Tags != null && 
                relation.Tags.TryGetValue("type", out actualValue) && 
                actualValue == "multipolygon")
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

                var points = ObjectPool.NewList<GeoCoordinate>();
                foreach (var outerWay in outerWays)
                    outerWay.FillPoints(points);

                // TODO process inner points!
                ModelVisitor.VisitArea(new Area
                {
                    Id = relation.Id,
                    Points = points,
                    Tags = relation.Tags
                });
            }
        }
    }
}
