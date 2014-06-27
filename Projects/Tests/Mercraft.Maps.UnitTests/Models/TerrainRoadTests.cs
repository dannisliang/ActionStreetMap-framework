using Mercraft.Core;
using Mercraft.Models.Terrain.Roads;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Models
{
    [TestFixture]
    public class TerrainRoadTests
    {
        [Test]
        public void CanBuildRoad()
        {
            // ARRANGE
            var roadBuilder = new RoadBuilder(new Vector2(0, 0), 512/500f, 512/500f);
            var roads = new[]
            {
                new Road
                {
                    ZIndex = 0,
                    SplatIndex = 0,
                    Width = 0.2f,
                    Points = new[]
                    {
                        new MapPoint(1, 1),
                        new MapPoint(7, 3),
                        new MapPoint(-7, 3)
                    }
                }
            };

            // ACT
            var alphaMapElements = roadBuilder.Build(roads);

            //ASSERT
            Assert.AreEqual(1, alphaMapElements.Length);
            Assert.AreEqual(7, alphaMapElements[0].Points.Length);
            // TODO check whether points are valid
        }
    }
}