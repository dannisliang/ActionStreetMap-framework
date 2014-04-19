using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Config;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private TexturePackProvider _textureProvider;
        private BuildingStyleProvider _styleProvider;

        [Dependency]
        public BuildingModelBuilder(TexturePackProvider textureProvider, BuildingStyleProvider styleProvider)
        {
            _textureProvider = textureProvider;
            _styleProvider = styleProvider;
        }

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
            
            // TODO define theme somewhere
            var theme = "berlin";
            var styleName = rule.GetBuildingStyle();

            var style = _styleProvider.Get(theme, styleName);
            var texture = _textureProvider.Get(style.Texture);

            gameObject.AddComponent<BuildingBehavior>()
                .Attach(RenderMode.Full, texture, style, height, levels, verticies);

            return gameObject;
        }

    }
}
