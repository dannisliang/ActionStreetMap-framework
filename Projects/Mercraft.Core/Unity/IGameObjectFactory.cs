using UnityEngine;

namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Creates GameObjects
    /// </summary>
    public interface IGameObjectFactory
    {
        IGameObject CreateNew();
        IGameObject CreatePrimitive(PrimitiveType type);

        // TODO add object pool logic?
    }
}