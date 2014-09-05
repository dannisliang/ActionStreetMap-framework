using Mercraft.Core.World.Roads;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestRoadBuilder: RoadBuilder
    {
        [Dependency]
        public TestRoadBuilder(IResourceProvider resourceProvider) : base(resourceProvider)
        {

        }

        protected override void CreateMesh(Road road, RoadStyle style)
        {
            // Do nothing
        }
    }
}
