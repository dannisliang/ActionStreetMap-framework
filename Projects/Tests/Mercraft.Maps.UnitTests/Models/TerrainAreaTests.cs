using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Models.Areas;
using Mercraft.Models.Terrain;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Models
{
    [TestFixture]
    public class TerrainAreaTests
    {
        [Test]
        public void CanBuildArea()
        {
            // ARRANGE
            var areaBuilder = new AreaBuilder(new Vector2(0, 0), 512/500f, 512/500f);
            var areas = new List<AreaSettings>()
            {
                new AreaSettings
                {
                    ZIndex = 0,
                    SplatIndex = 0,
                    Points = new[]
                    {
                        new MapPoint(1, 1),
                        new MapPoint(7, 3),
                        new MapPoint(-7, 3)
                    }
                }
            };

            // ACT
            var alphaMapElements = areaBuilder.Build(areas);

            // ASSERT
            Assert.AreEqual(1, alphaMapElements.Length);
            Assert.AreEqual(3, alphaMapElements[0].Points.Length);
        }
    }
}