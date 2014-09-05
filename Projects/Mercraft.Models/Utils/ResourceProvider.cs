using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Utils
{
    public interface IResourceProvider
    {
        Material GetMatertial(string key);
        Texture GetTexture(string key);
    }

    public class UnityResourceProvider : IResourceProvider
    {
        private Dictionary<string, Material> _materials = new Dictionary<string, Material>();
        private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        public Material GetMatertial(string key)
        {
            if (!_materials.ContainsKey(key))
                _materials[key] = Resources.Load<Material>(key);

            return _materials[key];
        }

        public Texture GetTexture(string key)
        {
            if (!_textures.ContainsKey(key))
                _textures[key] = Resources.Load<Texture>(key);

            return _textures[key];
        }
    }
}
