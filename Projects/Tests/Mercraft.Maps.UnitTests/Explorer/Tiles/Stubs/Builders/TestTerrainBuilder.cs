using System.Collections.Generic;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.Builders
{
    public class TestTerrainBuilder: TerrainBuilder
    {
        [Dependency]
        public TestTerrainBuilder(IGameObjectFactory gameObjectFactory, IResourceProvider resourceProvider,
            IRoadBuilder roadBuilder, IObjectPool objectPool)
            : base(gameObjectFactory, resourceProvider, roadBuilder, objectPool)
        {
        }

        protected override IGameObject CreateTerrainGameObject(IGameObject parent, TerrainSettings settings, 
            Vector3 size, List<int[,]> detailMapList)
        {
            return new TestGameObject();
        }
    }
}
