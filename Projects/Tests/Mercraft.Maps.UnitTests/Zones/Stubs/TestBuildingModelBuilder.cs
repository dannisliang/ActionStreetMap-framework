using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Config;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestBuildingModelBuilder : ModelBuilder
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly TexturePackProvider _textureProvider;
        private readonly BuildingStyleProvider _styleProvider;

        private string _theme = "berlin";
        private RenderMode _mode = RenderMode.Full;
    
        [Dependency]
        public TestBuildingModelBuilder(IGameObjectFactory goFactory,
            TexturePackProvider textureProvider, BuildingStyleProvider styleProvider)
        {
            _goFactory = goFactory;
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
            var gameObjectWrapper = _goFactory.CreateNew();

            var verticies = PolygonHelper.GetVerticies2D(center, footPrint);
            var height = rule.GetHeight(NoValue);
            var levels = rule.GetLevels(NoValue);

            var styleName = rule.GetBuildingStyle();

            var style = _styleProvider.Get(_theme, styleName);
            var texture = _textureProvider.Get(style.Texture);

         

            return gameObjectWrapper;
        }
    }
}