using Mercraft.Core;
using Mercraft.Maps.UnitTests.Models.Stubs;
using Mercraft.Models.Terrain;
using Mercraft.Models.Terrain.Areas;
using Mercraft.Models.Terrain.Roads;
using NUnit.Framework;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Models
{
    [TestFixture]
    public class TerrainAlphaMapGeneratorTests
    {
        [Test]
        public void CanProcessRealRoadsInCenterTile()
        {
            // ARRANGE & ACT
            var center = new Vector2(0, 0);
            var terrainData = ProcessRoads(center, 100f, CreateRealSettings(center));

            // ASSERT
            AssertNotEmptyness(terrainData);
        }

        [Test]
        public void CanProcessRealRoadsInRightTile()
        {
            // ARRANGE & ACT
            var center = new Vector2(100, 0);
            var terrainData = ProcessRoads(center, 100f, CreateRealSettings(center));

            // ASSERT
            AssertNotEmptyness(terrainData);
        }

        [Test]
        public void CanProcessFakeRoadInCenterTile()
        {
            // ARRANGE & ACT
            var center = new Vector2(0, 0);
            var terrainData = ProcessRoads(center, 10f, CreateFakeSettings(center));

            // ASSERT
            AssertNotEmptyness(terrainData);
        }

        [Test]
        public void CanProcessFakeRoadInRightTile()
        {
            // ARRANGE & ACT
            var center = new Vector2(10, 0);
            var terrainData = ProcessRoads(center, 10f, CreateFakeSettings(center));

            // ASSERT
            AssertNotEmptyness(terrainData);
        }

        public TestTerrainData ProcessRoads(Vector2 center, float size, TerrainSettings settings)
        {
            // ARRANGE
            var generator = new AlphaMapGenerator(settings);
            // NOTE unable to use moq here
            var terrainData = new TestTerrainData
            {
                Size = new Vector3(size, 1.0f, size)
            };

            // ACT
            generator.FillAlphaMap(terrainData);

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

        private TerrainSettings CreateRealSettings(Vector2 center)
        {
            return new TerrainSettings
            {
                CenterPosition = center,
                TerrainSize = 100,
                SplatPrototypes = new SplatPrototype[2],
                Areas = new Area[0],
                Roads = new[]
                {
                    new Road
                    {
                        ZIndex = 0.1f,
                        Width = 13,
                        SplatIndex = 1,
                        Points = new[]
                        {
                            new MapPoint(31.6f, 7.6f),
                            new MapPoint(40.7f, 8.8f),
                            new MapPoint(59.5f, 15.1f),
                            new MapPoint(91.0f, 24.5f),
                            new MapPoint(107.0f, 29.6f),
                            new MapPoint(116.6f, 33.7f),
                            new MapPoint(133.9f, 43.1f),
                            new MapPoint(142.4f, 46.0f),
                            new MapPoint(203.1f, 59.0f),
                            new MapPoint(236.3f, 64.1f),
                            new MapPoint(250.4f, 66.2f),
                            new MapPoint(276.0f, 70.3f),
                            new MapPoint(330.6f, 78.1f)
                        }
                    },
                    new Road
                    {
                        ZIndex = 0.1f,
                        Width = 13,
                        SplatIndex = 1,
                        Points = new[]
                        {
                            new MapPoint(-120.1f, -54.3f),
                            new MapPoint(-105.8f, -48.4f),
                            new MapPoint(-89.3f, -41.1f),
                            new MapPoint(-71.6f, -33.3f),
                            new MapPoint(10.0f, -0.5f),
                            new MapPoint(31.6f, 7.6f)
                        }
                    },
                    new Road
                    {
                        ZIndex = 0.1f,
                        Width = 3.5f,
                        SplatIndex = 1,
                        Points = new[]
                        {
                            new MapPoint(10.0f, -0.5f),
                            new MapPoint(33.9f, -53.9f),
                            new MapPoint(57.5f, -82.9f)
                        }
                    }
                }
            };
        }

        private TerrainSettings CreateFakeSettings(Vector2 center)
        {
            return new TerrainSettings
            {
                CenterPosition = center,
                TerrainSize = 10,
                SplatPrototypes = new SplatPrototype[2],
                Areas = new Area[0],
                Roads = new[]
                {
                    new Road
                    {
                        ZIndex = 0.1f,
                        Width = 1,
                        SplatIndex = 1,
                        Points = new[]
                        {
                            new MapPoint(0,0),
                            new MapPoint(10,0),
                        }
                    }
                }
            };
        }
    }
}