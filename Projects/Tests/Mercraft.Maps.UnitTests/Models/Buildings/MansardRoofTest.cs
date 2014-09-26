using Mercraft.Core;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Roofs;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Models.Buildings
{
    [TestFixture]
    public class MansardRoofTest
    {
        [Test]
        public void CanBuildMansardWithValidData()
        {
            // ARRANGE
            var builder = new MansardRoofBuilder();

            // ACT
            var meshData = builder.Build(new Building()
            {
                Footprint = new[]
                {
                    new MapPoint(0, 0),
                    new MapPoint(0, 5),
                    new MapPoint(5, 5),
                    new MapPoint(5, 0),
                },
                Elevation = 0,
                Height = 1
            }, new BuildingStyle()
            {
                Roof = new BuildingStyle.RoofStyle()
                {
                    Builders = new IRoofBuilder[] { new FlatRoofBuilder(), },
                    Materials = new string[] { ""},
                    Textures = new string[] { ""},
                },
                Facade = new BuildingStyle.FacadeStyle()
            });

            // ASSERT

            Assert.AreEqual(20, meshData.Vertices.Length);
            Assert.AreEqual(30, meshData.Triangles.Length);
            Assert.AreEqual(20, meshData.UV.Length);
        }
    }
}
