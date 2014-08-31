using Mercraft.Core.Elevation;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestBuildingBuilder: IBuildingBuilder
    {
        public void Build(HeightMap heightMap, Building building, BuildingStyle style)
        {
            Assert.IsNotNull(building);
            Assert.IsNotNull(style);
        }
    }
}
