using Mercraft.Core.World.Roads;
using Mercraft.Models.Roads;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    class TestRoadBuilder: IRoadBuilder
    {
        public void Build(Road road, RoadStyle style)
        {
            Assert.IsNotNull(road);
            Assert.IsNotNull(style);
        }
    }
}
