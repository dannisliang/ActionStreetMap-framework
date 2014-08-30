using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
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
using Mercraft.Models.Terrain;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private readonly IThemeProvider _themeProvider;
        private readonly IBuildingBuilder _builder;

        public override string Name
        {
            get { return "building"; }
        }

        [Dependency]
        public BuildingModelBuilder(IGameObjectFactory gameObjectFactory, 
            IThemeProvider themeProvider,
            IBuildingBuilder builder) :
            base(gameObjectFactory)
        {
            _themeProvider = themeProvider;
            _builder = builder;
        }

        private const int NoValue = 0;

        public override IGameObject BuildArea(GeoCoordinate center, HeightMap heightMap, Rule rule, Area area)
        {
            base.BuildArea(center, heightMap, rule, area);
            return BuildBuilding(center, heightMap, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, HeightMap heightMap, Rule rule, Way way)
        {
            base.BuildWay(center, heightMap, rule, way);
            return BuildBuilding(center, heightMap, way, way.Points, rule);
        }

        private IGameObject BuildBuilding(GeoCoordinate center, HeightMap heightMap, Model model, GeoCoordinate[] footPrint, Rule rule)
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
                Footprint = PolygonHelper.GetVerticies3D(center, heightMap, footPrint)
            };

            var theme = _themeProvider.Get();
            BuildingStyle style = theme.GetBuildingStyle(building);

            _builder.Build(building, style);

            return gameObjectWrapper;
        }
    }
}