using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core.Scene;
using Mercraft.Core.Utilities;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    public class RelationVisitor : ElementVisitor
    {
        //private readonly IScene _scene;
        public RelationVisitor(IScene scene) : base(scene)
        {
            // _scene = scene;
        }

        public override void VisitRelation(Relation relation)
        {

            /* if (relation.Tags.ContainsKeyValue("type", "multipolygon") ||
                relation.Tags.ContainsKeyValue("type", "building"))
            {
                var ways = new List<KeyValuePair<bool, Way>>();
                foreach (var member in relation.Members)
                {
                    var way = member.Member as Way;
                    if(way == null)
                        continue;

                    if (member.Role == "inner")
                    {
                        ways.Add(new KeyValuePair<bool, Way>(false, way));
                    }
                    else if(member.Role == "outer")
                    {
                        ways.Add(new KeyValuePair<bool, Way>(true, way));
                    }
                }
            }*/
        }
    }
}
