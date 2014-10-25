using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Buildings;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Buildings;
using Mercraft.Models.Utils;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides logic to build buildings.
    /// </summary>
    public class BuildingModelBuilder : ModelBuilder
    {
        private readonly IBuildingBuilder _builder;

        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();

        /// <inheritdoc />
        public override string Name
        {
            get { return "building"; }
        }

        /// <summary>
        ///     Creates BuildingModelBuilder.
        /// </summary>
        [Dependency]
        public BuildingModelBuilder(IBuildingBuilder builder)
        {
            _builder = builder;
        }

        private const int NoValue = 0;

        /// <inheritdoc />
        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);
            return BuildBuilding(tile, rule, area, area.Points);
        }

        private IGameObject BuildBuilding(Tile tile, Rule rule, Model model, List<GeoCoordinate> footPrint)
        {
            var points = ObjectPool.NewList<MapPoint>();
            PointHelper.GetClockwisePolygonPoints(tile.HeightMap, tile.RelativeNullPoint, footPrint, points);
            var minHeight = rule.GetMinHeight();

            var elevation = AdjustHeightMap(tile.HeightMap, points, minHeight);

            if (WorldManager.Contains(model.Id))
                return null;

            var gameObject = BuildGameObject(tile, rule, model, points, elevation, minHeight);

            ObjectPool.Store(points);

            return gameObject;
        }

        private float AdjustHeightMap(HeightMap heightMap, List<MapPoint> footPrint, float minHeight)
        {
            // TODO if we have added building to WorldManager then
            // we should use elevation from existing building

            var elevation = footPrint.Average(p => p.Elevation);

            for (int i = 0; i < footPrint.Count; i++)
                footPrint[i].SetElevation(elevation);
            // NOTE do not adjust height map in case of positive minHeight
            if (!heightMap.IsFlat && Math.Abs(minHeight) < 0.5f)
            {
                _heightMapProcessor.Recycle(heightMap);
                _heightMapProcessor.AdjustPolygon(footPrint, elevation);
                _heightMapProcessor.Clear();
            }
            return elevation;
        }

        private IGameObject BuildGameObject(Tile tile, Rule rule, Model model, List<MapPoint> points,
            float elevation, float minHeight)
        {
            var gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Building {0}", model));

            // NOTE observed that min_height should be subracted from height for building:part
            // TODO this should be done in mapcss, but stylesheet doesn't support multiply eval operations
            // on the same tag

            var height = rule.GetHeight(NoValue);
            if (rule.IsPart())
                height -= minHeight;

            // TODO should we save this object in WorldManager?
            var building = new Building
            {
                Id = model.Id,
                Address = AddressExtractor.Extract(model.Tags),
                GameObject = gameObjectWrapper,
                Height = height,
                Levels = rule.GetLevels(NoValue),
                MinHeight = minHeight,
                Type = rule.GetBuildingType(),
                RoofType = rule.GetRoofType(),
                FacadeColor = rule.GetFillColor(),
                FacadeMaterial = rule.GetFacadeMaterial(),
                RoofColor = rule.GetRoofColor(),
                RoofMaterial = rule.GetRoofMaterial(),
                Elevation = elevation, // we set equal elevation for every point
                Footprint = points,
            };

            var theme = ThemeProvider.Get();
            BuildingStyle style = theme.GetBuildingStyle(building);

            _builder.Build(tile.HeightMap, building, style);

            WorldManager.AddBuilding(building);

            return gameObjectWrapper;
        }
    }
}