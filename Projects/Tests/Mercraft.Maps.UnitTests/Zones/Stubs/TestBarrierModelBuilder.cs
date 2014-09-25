using System.Collections.Generic;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;
using Mercraft.Models.Utils.Lines;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
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
            DimensionLineBuilder.Build(tile.HeightMap, new List<LineElement>()
            {
                new LineElement(points, 0.2f)
            }, 
            (list, ints, arg3) => { });
            return null;
        }
    }
}
