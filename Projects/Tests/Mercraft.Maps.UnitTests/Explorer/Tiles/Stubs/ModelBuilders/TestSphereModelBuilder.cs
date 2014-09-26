using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestSphereModelBuilder: SphereModelBuilder
    {
        [Dependency]
        public TestSphereModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, IResourceProvider resourceProvider) :
            base(worldManager, gameObjectFactory, resourceProvider)
        {
        }

        protected override IGameObject BuildSphere(IGameObject gameObjectWrapper, Rule rule, Model model, MapPoint sphereCenter, float diameter,
            float minHeight)
        {
            // Do nothing
            return null;
        }
    }
}
