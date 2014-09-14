using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Utils
{
    public interface IResourceProvider
    {
        GameObject GetGameObject(string key);
        Material GetMatertial(string key);
        Texture GetTexture(string key);
        Texture2D GetTexture2D(string key);
    }

    public class UnityResourceProvider : IResourceProvider
    {
        private Dictionary<string, GameObject> _gameObjects = new Dictionary<string, GameObject>();
        private Dictionary<string, Material> _materials = new Dictionary<string, Material>();
        private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        private Dictionary<string, Texture2D> _textures2D = new Dictionary<string, Texture2D>();

        public GameObject GetGameObject(string key)
        {
            if (!_gameObjects.ContainsKey(key))
                _gameObjects[key] = Resources.Load<GameObject>(key);

            return _gameObjects[key];
        }

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

        public Texture2D GetTexture2D(string key)
        {
            if (!_textures2D.ContainsKey(key))
                _textures2D[key] = Resources.Load<Texture2D>(key);

            return _textures2D[key];
        }
    }
}
