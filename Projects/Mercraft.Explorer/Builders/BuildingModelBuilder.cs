using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using Mercraft.Models.Buildings;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private const int NoValue = 0;

        public override GameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildBuilding(center, area.Points, rule);
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            return BuildBuilding(center, way.Points, rule);
        }

        private GameObject BuildBuilding(GeoCoordinate center, GeoCoordinate[] footPrint, Rule rule)
        {
            var gameObject = new GameObject();

            var verticies = PolygonHelper.GetVerticies2D(center, footPrint);
            var height = rule.GetHeight(NoValue);
            var levels = rule.GetLevels(NoValue);
            var style = rule.GetBuildingStyle();
            gameObject.AddComponent<BuildingBehavior>()
                .Attach(style, height, levels, verticies);

            return gameObject;
        }

    }
}
