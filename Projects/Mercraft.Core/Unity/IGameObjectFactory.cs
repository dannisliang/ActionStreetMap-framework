
namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Creates GameObjects
    /// </summary>
    public interface IGameObjectFactory
    {
        // TODO add object pool logic?
        IGameObject CreateNew(string name);
        IGameObject CreateNew(string name, IGameObject parent);
        IGameObject CreatePrimitive(string name, UnityPrimitiveType type);

        IGameObject Wrap(string name, object gameObject);
    }
}