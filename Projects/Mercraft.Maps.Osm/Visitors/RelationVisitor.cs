using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core.Scene;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    public class RelationVisitor: ElementVisitor
    {
        public RelationVisitor(IScene scene) : base(scene)
        {
        }

        public override void VisitRelation(Relation relation)
        {
            // TODO
        }
    }
}
