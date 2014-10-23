using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
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
