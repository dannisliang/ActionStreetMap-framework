using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.Render
{
    public interface IMeshRenderer
    {
        string Name { get; }
        void Render(GameObject gameObject, Model model, Rule rule);
    }
}
