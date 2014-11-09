using Mercraft.Core;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Roofs;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Builders
{
    public class TestDomeRoofBuilder: DomeRoofBuilder
    {
        [Dependency]
        public TestDomeRoofBuilder(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        protected override void ProcessObject(IGameObject gameObjectWrapper, MapPoint center, float diameter, BuildingStyle style)
        {
            // Do nothing
        }
    }
}
