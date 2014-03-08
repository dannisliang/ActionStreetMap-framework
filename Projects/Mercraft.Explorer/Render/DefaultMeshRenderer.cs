using Mercraft.Infrastructure.Config;
using UnityEngine;

namespace Mercraft.Explorer.Render
{
    public class DefaultMeshRenderer: IMeshRenderer, IConfigurable
    {
        private const string MeshRenderNameKey = "@name";
        private const string MaterialPathKey = "material/@path";

        private string _materialPath;

        public string Name { get; private set; }

        public void Render(GameObject gameObject)
        {
            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.renderer.material = Resources.Load<Material>(_materialPath);
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString(MeshRenderNameKey);
            _materialPath = configSection.GetString(MaterialPathKey);
        }
    }
}
