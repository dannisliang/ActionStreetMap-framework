using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Details;
using Mercraft.Models.Terrain;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides the way to process trees.
    /// </summary>
    public class TreeModelBuilder : ModelBuilder
    {
        private readonly ITerrainBuilder _terrainBuilder;

        /// <inheritdoc />
        public override string Name
        {
            get { return "tree"; }
        }

        /// <summary>
        ///     Creates TreeModelBuilder.
        /// </summary>
        [Dependency]
        public TreeModelBuilder(ITerrainBuilder terrainBuilder)
        {
            _terrainBuilder = terrainBuilder;
        }

        /// <inheritdoc />
        public override IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            var mapPoint = GeoProjection.ToMapCoordinate(tile.RelativeNullPoint, node.Point);
            mapPoint.Elevation = tile.HeightMap.LookupHeight(mapPoint);

            _terrainBuilder.AddTree(new TreeDetail
            {
                Id = node.Id,
                Point = mapPoint
            });

            return null;
        }
    }
}