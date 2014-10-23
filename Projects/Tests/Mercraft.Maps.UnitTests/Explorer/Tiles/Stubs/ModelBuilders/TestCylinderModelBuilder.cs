using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    public class TestCylinderModelBuilder : CylinderModelBuilder
    {
        protected override IGameObject BuildCylinder(IGameObject gameObjectWrapper, Rule rule, Model model, MapPoint cylinderCenter, float diameter,
            float actualHeight, float minHeight)
        {
            // do nothing
            return null;
        }
    }
}