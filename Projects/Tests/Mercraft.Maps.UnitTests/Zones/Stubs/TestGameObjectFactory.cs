using Mercraft.Core.Unity;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestGameObjectFactory : GameObjectFactory
    {
        [Dependency]
        public TestGameObjectFactory(ITerrainBuilder terrainBuilder) : base(terrainBuilder)
        {
        }

        public override IGameObject CreateNew(string name)
        {
            return new TestGameObject();
        }

        public override IGameObject CreatePrimitive(string name, UnityPrimitiveType type)
        {
            return new TestGameObject();
        }
    }
}