using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Models.Terrain;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestWaterModelBuilder : WaterModelBuilder
    {
        [Dependency]
        public TestWaterModelBuilder(ITerrainBuilder terrainBuilder, WorldManager worldManager, IGameObjectFactory gameObjectFactory,
            IResourceProvider resourceProvider, IObjectPool objectPool)
            : base(terrainBuilder, worldManager, gameObjectFactory, resourceProvider, objectPool)
        {
        }

        protected override void BuildObject(IGameObject gameObjectWrapper, Rule rule, Vector3[] points, int[] triangles)
        {
            // do nothing
        }
    }
}
