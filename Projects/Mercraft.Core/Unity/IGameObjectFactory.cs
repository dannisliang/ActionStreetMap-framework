using UnityEngine;

namespace Mercraft.Core.Unity
{
    public interface IGameObjectFactory
    {
        IGameObject CreateNew();
        IGameObject CreatePrimitive(PrimitiveType type);
    }
}
