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
    public class TestCylinderModelBuilder : CylinderModelBuilder
    {
        [Dependency]
        public TestCylinderModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, IResourceProvider resourceProvider) : base(worldManager, gameObjectFactory, resourceProvider)
        {
        }

        protected override IGameObject BuildCylinder(IGameObject gameObjectWrapper, Rule rule, Model model, MapPoint cylinderCenter, float diameter,
            float actualHeight, float minHeight)
        {
            // do nothing
            return null;
        }
    }
}