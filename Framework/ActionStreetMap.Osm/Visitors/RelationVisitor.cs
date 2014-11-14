using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Utilities;
using ActionStreetMap.Osm.Entities;
using Way = ActionStreetMap.Osm.Entities.Way;

namespace ActionStreetMap.Osm.Visitors
{
    /// <summary>
    ///     Relation visitor.
    /// </summary>
    public class RelationVisitor : ElementVisitor
    {
        /// <inheritdoc />
        public RelationVisitor(IModelVisitor modelVisitor, IObjectPool objectPool)
            : base(modelVisitor, objectPool)
        {
        }

        /// <inheritdoc />
        public override void VisitRelation(Relation relation)
        {
            // see http://wiki.openstreetmap.org/wiki/Relation:multipolygon
            string actualValue;
            if (relation.Tags != null && 
                relation.Tags.TryGetValue("type", out actualValue) && 
                actualValue == "multipolygon")
            {
                var innerWays = new List<Entities.Way>();
                var outerWays = new List<Entities.Way>();
                foreach (var member in relation.Members)
                {
                    var way = member.Member as Entities.Way;
                    if(way == null)
                        continue;

                    if (member.Role == "inner")
                        innerWays.Add(way);
                    else if(member.Role == "outer")
                        outerWays.Add(way);
                }

                if (!outerWays.Any())
                    return;
               
                foreach (var outerWay in outerWays)
                {
                    // TODO log this!
                    if (!outerWay.IsPolygon)
                        continue;

                    // TODO process inner points!
                    // NOTE inner points are representing holes in area
                    
                    var points = ObjectPool.NewList<GeoCoordinate>();
                    outerWay.FillPoints(points);
                    ModelVisitor.VisitArea(new Area
                    {
                        Id = outerWay.Id,
                        Points = points,
                        Tags = MergeTags(relation.Tags, outerWay.Tags)
                    });
                }
            }
        }

        private static Dictionary<string, string> MergeTags(Dictionary<string, string> first, Dictionary<string, string> second)
        {
            var dict = first == null ? 
                new Dictionary<string, string>() : 
                new Dictionary<string, string>(first);

            if (second != null)
            {
                foreach (var key in second.Keys)
                {
                    if (!dict.ContainsKey(key))
                        dict.Add(key, second[key]);
                }
            }

            return dict;
        }
    }
}
