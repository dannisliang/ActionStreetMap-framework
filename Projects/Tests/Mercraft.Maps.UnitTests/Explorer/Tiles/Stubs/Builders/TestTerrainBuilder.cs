using System.Collections.Generic;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Builders
{
    public class TestTerrainBuilder: TerrainBuilder
    {
        [Dependency]
        public TestTerrainBuilder(IResourceProvider resourceProvider) : base(resourceProvider)
        {
        }

        protected override IGameObject CreateTerrainGameObject(IGameObject parent, TerrainSettings settings, 
            Vector3 size, float[,] htmap, List<int[,]> detailMapList)
        {
            return new TestGameObject();
        }
    }
}
