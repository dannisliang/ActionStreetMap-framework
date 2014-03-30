using UnityEngine;

namespace Mercraft.Explorer.Interactions
{
    public interface IModelBehaviour
    {
        string Name { get; }
        void Apply(GameObject gameObject);
    }
}
