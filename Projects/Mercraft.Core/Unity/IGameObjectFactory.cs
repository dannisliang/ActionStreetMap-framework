
namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Creates GameObjects.
    /// </summary>
    public interface IGameObjectFactory
    {
        // TODO add object pool logic?

        /// <summary>
        ///     Creates new game object with given name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject CreateNew(string name);

        /// <summary>
        ///     Creates new game object with given name and parent.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="parent">Parent.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject CreateNew(string name, IGameObject parent);

        /// <summary>
        ///     Creates new game object primitive with given name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="type">Primitive type.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject CreatePrimitive(string name, UnityPrimitiveType type);

        /// <summary>
        ///     Wraps existing Unity's game object.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="gameObject">Unity's GameObject.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject Wrap(string name, object gameObject);
    }
}