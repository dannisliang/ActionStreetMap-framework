using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Extensions;
using Mercraft.Models;
using Mercraft.Models.Scenas;

namespace Mercraft.Maps.Osm.Visitors
{
    public class BuildingVisitor: IElementVisitor
    {
        private IScene _scene;

        public BuildingVisitor(IScene scene)
        {
            _scene = scene;
        }


        public void VisitNode(Node node)
        {
        }

        public void VisitWay(Way way)
        {
            if (!IsBuilding(way.Tags)) 
                return;

            var building = new Building();
            _scene.AddBuilding(building);
        }

        public void VisitRelation(Relation relation)
        { 
        }


        private bool IsBuilding(ICollection<Tag> tags)
        {
            return tags.ContainsKey("building") && !tags.IsFalse("building");
        }

    }
}
