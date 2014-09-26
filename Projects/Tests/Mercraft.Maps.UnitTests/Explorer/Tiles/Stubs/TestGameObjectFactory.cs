using Mercraft.Core.Unity;
using Mercraft.Explorer.Infrastructure;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
{
    public class TestGameObjectFactory : GameObjectFactory
    {
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