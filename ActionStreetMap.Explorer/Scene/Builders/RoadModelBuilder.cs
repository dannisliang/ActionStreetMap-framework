﻿using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.MapCss.Domain;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Core.Scene.World.Roads;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Osm.Helpers;
using ActionStreetMap.Models.Geometry;
using ActionStreetMap.Models.Terrain;
using ActionStreetMap.Explorer.Helpers;

namespace ActionStreetMap.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides the way to process roads.
    /// </summary>
    public class RoadModelBuilder: ModelBuilder
    {
        private readonly ITerrainBuilder _terrainBuilder;

        /// <inheritdoc />
        public override string Name
        {
            get { return "road"; }
        }

        /// <summary>
        ///     Creates RoadModelBuilder.
        /// </summary>
        [Dependency]
        public RoadModelBuilder(ITerrainBuilder terrainBuilder)
        {
            _terrainBuilder = terrainBuilder;
        }

        /// <inheritdoc />
        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            var points = ObjectPool.NewList<MapPoint>();
            PointUtils.FillHeight(tile.HeightMap, tile.RelativeNullPoint, way.Points, points);

            // road should be processed in one place: it's better to collect all 
            // roads and create connected road network
            _terrainBuilder.AddRoadElement(new RoadElement
            {
                Id = way.Id,
                Address = AddressExtractor.Extract(way.Tags),
                Width = (int) Math.Round(rule.GetWidth() / 2),
                ZIndex = rule.GetZIndex(),
                Points = points
            });

            return null;
        }
    }
}
