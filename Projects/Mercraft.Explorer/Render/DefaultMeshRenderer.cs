using System;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.Render
{
    public class DefaultMeshRenderer: IMeshRenderer, IConfigurable
    {
        private Shader _shader;
        private Color _color;

        public string Name { get; private set; }

        [Dependency]
        public DefaultMeshRenderer()
        {
            
        }

        public DefaultMeshRenderer(Shader shader, Color color)
        {
            _shader = shader;
            _color = color;
        }

        public void Render(GameObject gameObject)
        {
            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material.shader = _shader;

            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, _color);
            tex.Apply();
            renderer.material.mainTexture = tex;
            renderer.material.color = _color;
        }

        public void Configure(IConfigSection configSection)
        {
            Name = configSection.GetString("@name");

            _shader = Shader.Find(configSection.GetString("shader"));

            var colorString = configSection.GetString("color");
            _color = Color.green;
            //(Color)Enum.Parse(typeof(Color), colorString);
        }
    }
}
