using UnityEngine;

namespace Mercraft.Explorer.Render
{
    public interface IMeshRenderer
    {
        string Name { get; }
        void Render(GameObject gameObject);
    }
}
