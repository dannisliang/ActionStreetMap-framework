using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Buildings;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private readonly IThemeProvider _themeProvider;
        private readonly IBuildingStyleProvider _buildingStyleProvider;
        private readonly IBuildingBuilder _builder;

        [Dependency]
        public BuildingModelBuilder(IGameObjectFactory gameObjectFactory, 
            IThemeProvider themeProvider,
            IBuildingStyleProvider buildingStyleProvider,
            IBuildingBuilder builder) :
            base(gameObjectFactory)
        {
            _themeProvider = themeProvider;
            _buildingStyleProvider = buildingStyleProvider;
            _builder = builder;
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
            var gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Building {0}", model));
            
            // TODO should we save this object in WorldManager?
            var building = new Building()
            {
                Address = AddressExtractor.Extract(model.Tags),
                GameObject = gameObjectWrapper,
                Height = rule.GetHeight(NoValue),
                Levels = rule.GetLevels(NoValue),
                // TODO map osm type to ours
                Type = "residental",//rule.GetBuildingType(),
                BottomOffset = rule.GetZIndex(),
                Footprint = PolygonHelper.GetVerticies2D(center, footPrint)
            };

            var theme = _themeProvider.Get();
            BuildingStyle style = _buildingStyleProvider.Get(theme, building);

            _builder.Build(building, style);

            return gameObjectWrapper;
        }
    }
}