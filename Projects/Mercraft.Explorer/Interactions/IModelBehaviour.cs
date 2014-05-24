using Mercraft.Core;
using Mercraft.Core.Unity;

namespace Mercraft.Explorer.Interactions
{
    public interface IModelBehaviour
    {
        string Name { get; }
        void Apply(IGameObject gameObject);
    }
}
