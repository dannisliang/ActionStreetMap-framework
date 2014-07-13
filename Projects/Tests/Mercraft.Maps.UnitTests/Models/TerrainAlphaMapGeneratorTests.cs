using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Maps.UnitTests.Models.Stubs;
using Mercraft.Models.Areas;
using Mercraft.Models.Terrain;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Models
{
    [TestFixture]
    public class TerrainAlphaMapGeneratorTests
    {

        [Test]
        public void CanProcessFakeRoadInCenterTile()
        {
            // ARRANGE & ACT
            var center = new Vector2(0, 0);
            var terrainData = ProcessAreas(center, 10f, CreateFakeSettings(center));

            // ASSERT
            AssertNotEmptyness(terrainData);
        }

        [Test]
        public void CanProcessFakeRoadInRightTile()
        {
            // ARRANGE & ACT
            var center = new Vector2(10, 0);
            var terrainData = ProcessAreas(center, 10f, CreateFakeSettings(center));

            // ASSERT
            AssertNotEmptyness(terrainData);
        }

        public TestTerrainData ProcessAreas(Vector2 center, float size, TerrainSettings settings)
        {
            // ARRANGE
            var generator = new AlphaMapGenerator(settings);
            // NOTE unable to use moq here
            var terrainData = new TestTerrainData
            {
                Size = new Vector3(size, 1.0f, size)
            };

            // ACT
            terrainData.SetAlphamaps(0, 0, generator.GetAlphaMap(terrainData));

            return terrainData;
        }

        private void AssertNotEmptyness(TestTerrainData terrainData)
        {
            var notZero = 0;
            for (int i = 0; i < terrainData.AlphamapResolution; i++)
            {
                for (int j = 0; j < terrainData.AlphamapResolution; j++)
                {
                    if (terrainData.Map[i, j, 1] > 0)
                    {
                        notZero++;
                    }
                }
            }
            Assert.Greater(notZero, 0);
        }


        private TerrainSettings CreateFakeSettings(Vector2 center)
        {
            return new TerrainSettings
            {
                CenterPosition = center,
                TerrainSize = 10,
                SplatPrototypes = new SplatPrototype[2],
                Areas = new List<AreaSettings>
                {
                    new AreaSettings
                    {
                        ZIndex = 0.1f,
                        SplatIndex = 1,
                        Points = new[]
                        {
                            new MapPoint(0,0),
                            new MapPoint(10,0),
                            new MapPoint(10,10),
                        }
                    }
                }
            };
        }
    }
}