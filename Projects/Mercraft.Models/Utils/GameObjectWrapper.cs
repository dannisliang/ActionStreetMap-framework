using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Models.Utils
{
    internal class GameObjectWrapper : IGameObject
    {
        private readonly GameObject _gameObject;

        public GameObjectWrapper(string name, GameObject gameObject)
        {
            _gameObject = gameObject;
            _gameObject.name = name;
        }

        public T GetComponent<T>()
        {
            if (!typeof(T).IsAssignableFrom(typeof (GameObject)))
                return (T) (object)_gameObject.GetComponent(typeof(T));
            
            // This is workaround to make code unit-tesable outside Unity context
            return (T) (object) _gameObject;
        }

        public string Name { get; set; }
        public IGameObject Parent { set; private get; }
    }
}