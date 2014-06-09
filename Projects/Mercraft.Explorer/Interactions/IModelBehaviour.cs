using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;

namespace Mercraft.Explorer.Interactions
{
    public interface IModelBehaviour
    {
        string Name { get; }
        void Apply(IGameObject gameObject, Model model);
    }
}