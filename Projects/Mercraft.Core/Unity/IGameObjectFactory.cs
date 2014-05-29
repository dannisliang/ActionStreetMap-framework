using UnityEngine;

namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Creates GameObjects
    /// </summary>
    public interface IGameObjectFactory
    {
        IGameObject CreateNew(string name);
        IGameObject CreatePrimitive(string name, PrimitiveType type);

        // TODO add object pool logic?
    }
}