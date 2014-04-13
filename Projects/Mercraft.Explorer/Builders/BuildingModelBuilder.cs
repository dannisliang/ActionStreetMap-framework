using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Models.Buildings;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        public override GameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildBuilding(center, area.Points, area, rule);
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            return BuildBuilding(center, way.Points, way, rule);
        }

        private GameObject BuildBuilding(GeoCoordinate center, GeoCoordinate[] footPrint, Model model, Rule rule)
        {
            var gameObject = new GameObject();
            gameObject.name = String.Format("Building {0}", model);

             var verticies = PolygonHelper.GetVerticies2D(center, footPrint);
             gameObject.AddComponent<BuildingBehavior>().Attach(verticies);

             return gameObject;
        }
    }
}
