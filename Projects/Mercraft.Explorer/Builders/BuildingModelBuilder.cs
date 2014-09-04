using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
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

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);
            return BuildBuilding(tile, area, area.Points, rule);
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            base.BuildWay(tile, rule, way);
            return BuildBuilding(tile, way, way.Points, rule);
        }

        private IGameObject BuildBuilding(Tile tile, Model model, GeoCoordinate[] footPrint, Rule rule)
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
                Elevation = rule.GetZIndex(),
                Footprint = PolygonHelper.GetVerticies3D(tile.RelativeNullPoint, tile.HeightMap, footPrint)
            };

            var theme = _themeProvider.Get();
            BuildingStyle style = theme.GetBuildingStyle(building);

            _builder.Build(tile.HeightMap, building, style);

            return gameObjectWrapper;
        }
    }
}