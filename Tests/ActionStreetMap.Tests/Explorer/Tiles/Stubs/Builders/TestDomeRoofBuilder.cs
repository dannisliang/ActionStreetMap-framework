using ActionStreetMap.Core;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Models.Buildings;
using ActionStreetMap.Models.Buildings.Roofs;

namespace ActionStreetMap.Tests.Explorer.Tiles.Stubs.Builders
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
