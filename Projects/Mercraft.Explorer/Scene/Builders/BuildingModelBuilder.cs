using System;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Core.World.Buildings;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Buildings;
using Mercraft.Models.Utils;

namespace Mercraft.Explorer.Scene.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private readonly IThemeProvider _themeProvider;
        private readonly IBuildingBuilder _builder;

        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();

        public override string Name
        {
            get { return "building"; }
        }

        [Dependency]
        public BuildingModelBuilder(WorldManager worldManager,
            IGameObjectFactory gameObjectFactory, 
            IThemeProvider themeProvider,
            IBuildingBuilder builder) :
            base(worldManager, gameObjectFactory)
        {
            _themeProvider = themeProvider;
            _builder = builder;
        }

        private const int NoValue = 0;

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);
            return BuildBuilding(tile, rule, area, area.Points);
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            base.BuildWay(tile, rule, way);
            return BuildBuilding(tile, rule, way, way.Points);
        }

        private IGameObject BuildBuilding(Tile tile, Rule rule, Model model, GeoCoordinate[] footPrint)
        {
            var points = PolygonHelper.GetVerticies3D(tile.RelativeNullPoint, tile.HeightMap, footPrint);

            AdjustHeightMap(tile.HeightMap, points);

            if (WorldManager.Contains(model.Id))
                return null;
                
            return BuildGameObject(tile, rule, model, points);
        }

        private void AdjustHeightMap(HeightMap heightMap, MapPoint[] footPrint)
        {
            // TODO if we have added building to WorldManager then
            // we should use elevation from existing building

            var elevation = footPrint.Average(p => p.Elevation);

            for (int i = 0; i < footPrint.Length; i++)
                footPrint[i].Elevation = elevation;

            if (!heightMap.IsFlat)
            {
                _heightMapProcessor.Recycle(heightMap);
                _heightMapProcessor.AdjustPolygon(footPrint, elevation);
            }
        }

        private IGameObject BuildGameObject(Tile tile, Rule rule, Model model, MapPoint[] points)
        {
            var gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Building {0}", model));

            // NOTE observed that min_height should be subracted from height for building:part
            // TODO this should be done in mapcss, but stylesheet doesn't support multiply eval operations
            // on the same tag
            var minHeight = rule.GetMinHeight();
            var height = rule.GetHeight(NoValue);
            if (rule.IsPart())
                height -= minHeight;

            // TODO should we save this object in WorldManager?
            var building = new Building()
            {
                Id = model.Id,
                Address = AddressExtractor.Extract(model.Tags),
                GameObject = gameObjectWrapper,
                Height = height,
                Levels = rule.GetLevels(NoValue),
                MinHeight = minHeight,
                Type = rule.GetBuildingType(),
                FacadeColor = rule.GetFillColor(),
                FacadeMaterial = rule.GetFacadeMaterial(),
                Elevation = points[0].Elevation, // we set equal elevation for every point
                Footprint = points,
            };

            var theme = _themeProvider.Get();
            BuildingStyle style = theme.GetBuildingStyle(building);

            _builder.Build(tile.HeightMap, building, style);

            WorldManager.AddBuilding(building);

            return gameObjectWrapper;
        }
    }
}