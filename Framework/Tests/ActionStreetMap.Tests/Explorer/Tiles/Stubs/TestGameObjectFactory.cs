using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Infrastructure;

namespace ActionStreetMap.Maps.UnitTests.Explorer.Tiles.Stubs
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