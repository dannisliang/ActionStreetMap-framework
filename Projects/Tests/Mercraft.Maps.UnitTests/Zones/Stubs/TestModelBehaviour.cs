using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
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
