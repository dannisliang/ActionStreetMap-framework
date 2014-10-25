using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides the way to process splat areas.
    /// </summary>
    public class SplatModelBuilder : ModelBuilder
    {
        private readonly ITerrainBuilder _terrainBuilder;

        /// <inheritdoc />
        public override string Name
        {
            get { return "splat"; }
        }

        /// <summary>
        ///     Creates TreeModelBuilder.
        /// </summary>
        [Dependency]
        public SplatModelBuilder(ITerrainBuilder terrainBuilder)
        {
            _terrainBuilder = terrainBuilder;
        }

        /// <inheritdoc />
        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            var points = ObjectPool.NewList<MapPoint>();
            PointHelper.GetPolygonPoints(tile.RelativeNullPoint, area.Points, points);
            _terrainBuilder.AddArea(new AreaSettings
            {
                ZIndex = rule.GetZIndex(),
                SplatIndex = rule.GetSplatIndex(),
                DetailIndex = rule.GetTerrainDetailIndex(),
                Points = points
            });

            return null;
        }
    }
}
