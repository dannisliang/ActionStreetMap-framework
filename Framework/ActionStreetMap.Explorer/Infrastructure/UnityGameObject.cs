using ActionStreetMap.Core.Unity;
using UnityEngine;

namespace ActionStreetMap.Explorer.Infrastructure
{
    /// <summary>
    ///     Wrapper of real Unity's GameObject.
    /// </summary>
    public class UnityGameObject : IGameObject
    {
        private readonly GameObject _gameObject;

        /// <summary>
        ///     Creates UnityGameObject. Internally creates Unity's GameObject with given name.
        /// </summary>
        /// <param name="name">Name.</param>
        public UnityGameObject(string name)
        {
            _gameObject = new GameObject(name);
        }

        /// <summary>
        ///     Creates UnityGameObject.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="gameObject">GameObject to be wrapperd.</param>
        public UnityGameObject(string name, GameObject gameObject)
        {
            _gameObject = gameObject;
            _gameObject.name = name;
        }

        /// <inheritdoc />
        public T GetComponent<T>()
        {
            // This is workaround to make code unit-tesable outside Unity context
            return (T) (object) _gameObject;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _gameObject.name; }
            set { _gameObject.name = value; }
        }

        /// <inheritdoc />
        public IGameObject Parent
        {
            set { _gameObject.transform.parent = value.GetComponent<GameObject>().transform; }
        }
    }
}