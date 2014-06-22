using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;

namespace Mercraft.Core.Scene
{
    public interface IModelBehaviour
    {
        string Name { get; }
        void Apply(IGameObject gameObject, Model model);
    }
}