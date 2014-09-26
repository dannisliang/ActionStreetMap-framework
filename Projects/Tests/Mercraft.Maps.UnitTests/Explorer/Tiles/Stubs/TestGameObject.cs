using Mercraft.Core.Unity;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
{
    class TestGameObject: IGameObject
    {
        public T GetComponent<T>()
        {
            return default(T);
        }

        public string Name { get; set; }
        public IGameObject Parent { set; private get; }
    }
}
