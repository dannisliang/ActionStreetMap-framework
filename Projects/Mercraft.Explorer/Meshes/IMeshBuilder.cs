
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.Meshes
{
    public interface IMeshBuilder
    {
        string Name { get; }
        Mesh Build(Vector2[] verticies, Model model, Rule rule);
    }
}
