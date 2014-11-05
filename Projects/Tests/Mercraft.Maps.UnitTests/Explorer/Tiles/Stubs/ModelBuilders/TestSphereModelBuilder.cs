using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestSphereModelBuilder: SphereModelBuilder
    {
        protected override IGameObject BuildSphere(IGameObject gameObjectWrapper, Rule rule, Model model, MapPoint sphereCenter, float diameter,
            float heightOffset)
        {
            // Do nothing
            return null;
        }
    }
}
