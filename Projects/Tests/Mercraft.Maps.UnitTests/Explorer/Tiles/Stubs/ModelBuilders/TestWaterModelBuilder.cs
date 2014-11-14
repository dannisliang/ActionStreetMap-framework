using ActionStreetMap.Core.MapCss.Domain;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Scene.Builders;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Models.Terrain;
using UnityEngine;

namespace ActionStreetMap.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestWaterModelBuilder : WaterModelBuilder
    {
        [Dependency]
        public TestWaterModelBuilder(ITerrainBuilder terrainBuilder)
            : base(terrainBuilder)
        {
        }

        protected override void BuildObject(IGameObject gameObjectWrapper, Rule rule, Vector3[] points, int[] triangles)
        {
            // do nothing
        }
    }
}
