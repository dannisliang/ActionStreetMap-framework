using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Interactions;
using Mercraft.Infrastructure.Config;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestModelBehavior : IModelBehaviour, IConfigurable
    {
        private const string NameKey = "@name";
        public string Name { get; private set; }

        public void Apply(IGameObject gameObject, Model model)
        {
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(NameKey);
        }
    }
}
