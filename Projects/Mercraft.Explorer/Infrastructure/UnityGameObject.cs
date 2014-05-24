using System;
using Mercraft.Core;
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

        public UnityGameObject()
        {
            _gameObject = new GameObject();
        }

        public UnityGameObject(GameObject gameObject)
        {
            _gameObject = gameObject;
        }


        public T GetComponent<T>()
        {
            // This is workaround to make code unit-tesable outside Unity context
            return (T) (object)_gameObject;
        }
    }
}