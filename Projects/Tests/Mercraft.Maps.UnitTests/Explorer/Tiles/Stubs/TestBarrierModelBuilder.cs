using System.Collections.Generic;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Geometry.ThickLine;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
{
    public class TestBarrierModelBuilder: BarrierModelBuilder
    {
        [Dependency]
        public TestBarrierModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, 
            IResourceProvider resourceProvider) : base(worldManager, gameObjectFactory, resourceProvider)
        {
        }

        public override string Name
        {
            get { return "barrier"; }
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            var points = PolygonHelper.GetVerticies3D(tile.RelativeNullPoint, tile.HeightMap, way.Points);
            DimenLineBuilder.Build(tile.HeightMap, new List<LineElement>()
            {
                new LineElement(points, 0.2f)
            }, 
            (list, ints, arg3) => { });
            return null;
        }
    }
}
