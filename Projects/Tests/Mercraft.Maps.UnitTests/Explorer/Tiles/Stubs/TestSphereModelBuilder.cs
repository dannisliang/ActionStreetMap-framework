using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
{
    class TestSphereModelBuilder: SphereModelBuilder
    {
        [Dependency]
        public TestSphereModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, IResourceProvider resourceProvider) :
            base(worldManager, gameObjectFactory, resourceProvider)
        {
        }

        protected override IGameObject BuildSphere(Tile tile, Model model, GeoCoordinate[] points, Rule rule)
        {
            return null;
        }
    }
}
