using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Explorer.Infrastructure
{
    /// <summary>
    ///     Decorator of real Unity's GameObject
    /// </summary>
    public class UnityGameObject : IGameObject
    {
        private readonly GameObject _gameObject;

        public UnityGameObject(string name)
        {
            _gameObject = new GameObject(name);
        }

        public UnityGameObject(string name, GameObject gameObject)
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