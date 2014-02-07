using System;
using System.Linq;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Models;

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
            if (!IsBuilding(way)) 
                return;

            var building = new Building();
            _scene.AddBuilding(building);
        }

        public void VisitRelation(Relation relation)
        { 
        }


        private bool IsBuilding(Way way)
        {
            return way.Tags.Any(t =>
                t.Key.Equals("building", StringComparison.OrdinalIgnoreCase) &&
                t.Value.Equals("yes", StringComparison.OrdinalIgnoreCase));
        }

    }
}
