using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Core.Unity;

namespace ActionStreetMap.Maps.UnitTests.Explorer.Tiles.Stubs.Behaviours
{
    public class TestModelBehaviour : IModelBehaviour
    {
        public string Name { get; private set; }

        public TestModelBehaviour(string name)
        {
            Name = name;
        }
        public void Apply(IGameObject gameObject, Model model)
        {
            
        }
    }
}
