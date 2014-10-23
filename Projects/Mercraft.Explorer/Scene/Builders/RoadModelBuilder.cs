using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Terrain;

namespace Mercraft.Explorer.Scene.Builders
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
            PointHelper.FillHeight(tile.RelativeNullPoint, tile.HeightMap, way.Points, points, way.Points.Count);

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
