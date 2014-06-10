using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Config;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private const string RenderModeKey = @"render/@mode";
        private const string ThemeKey = @"render/@theme";
        private readonly TexturePackProvider _textureProvider;
        private readonly BuildingStyleProvider _styleProvider;

        private string _theme;

        [Dependency]
        public BuildingModelBuilder(IGameObjectFactory goFactory,
            TexturePackProvider textureProvider, BuildingStyleProvider styleProvider):
            base(goFactory)
        {
            _textureProvider = textureProvider;
            _styleProvider = styleProvider;
        }

        private const int NoValue = 0;

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildBuilding(center, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            return BuildBuilding(center, way, way.Points, rule);
        }

        private IGameObject BuildBuilding(GeoCoordinate center, Model model, GeoCoordinate[] footPrint, Rule rule)
        {
            var gameObjectWrapper = _goFactory.CreateNew(String.Format("Building {0}", model));
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            var verticies = PolygonHelper.GetVerticies2D(center, footPrint);
            var height = rule.GetHeight(NoValue);
            var levels = rule.GetLevels(NoValue);

            var styleName = rule.GetBuildingStyle();

            var style = _styleProvider.Get(_theme, styleName);
            var texture = _textureProvider.Get(style.Texture);

            gameObject.AddComponent<BuildingBehavior>().Attach(
                new BuildingSettings
                {
                    Seed = model.Id,
                    Height = height,
                    Levels = levels,
                    Style = style,
                    TexturePack = texture,
                    FootPrint = verticies.ToVector2()
                });

            return gameObjectWrapper;
        }

        public override void Configure(IConfigSection configSection)
        {
            base.Configure(configSection);
            _theme = configSection.GetString(ThemeKey);
        }
    }
}