using Mercraft.Core.Unity;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestGameObjectFactory : IGameObjectFactory
    {
        public IGameObject CreateNew(string name)
        {
            return new TestGameObject();
        }

        public IGameObject CreatePrimitive(string name, UnityPrimitiveType type)
        {
            return new TestGameObject();
        }
    }
}