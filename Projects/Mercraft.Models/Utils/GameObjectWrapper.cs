using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Models.Utils
{
    public class GameObjectWrapper : IGameObject
    {
        private readonly GameObject _gameObject;

        public GameObjectWrapper(string name, GameObject gameObject)
        {
            _gameObject = gameObject;
            _gameObject.name = name;
        }

        public T GetComponent<T>()
        {
            // This is workaround to make code unit-tesable outside Unity context
            return (T) (object) _gameObject;
        }
    }
}