using System;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Config;
using UnityEngine;

namespace Mercraft.Explorer.Render
{
    public class DefaultMeshRenderer: IMeshRenderer, IConfigurable
    {
        private const string MeshRenderNameKey = "@name";

        public string Name { get; private set; }

        public void Render(GameObject gameObject, Model model, Rule rule)
        {
            var material = rule.Evaluate<string>(model, "material");
            var materialPath = String.Format("Materials/{0}", material);

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.renderer.material = Resources.Load<Material>(materialPath);
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(MeshRenderNameKey);
        }
    }
}
