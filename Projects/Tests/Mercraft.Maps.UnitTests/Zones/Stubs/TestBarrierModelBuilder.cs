using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestBarrierModelBuilder: ModelBuilder
    {
        public override string Name
        {
            get { return "barrier"; }
        }

        [Dependency]
        public TestBarrierModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory) :
            base(worldManager, gameObjectFactory)
        {
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            return null;
        }
    }
}
