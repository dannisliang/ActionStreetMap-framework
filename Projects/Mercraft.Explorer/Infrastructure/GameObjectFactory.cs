using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Explorer.Infrastructure
{
    public class GameObjectFactory : IGameObjectFactory
    {
        public virtual IGameObject CreateNew()
        {
            return new UnityGameObject();
        }

        public virtual IGameObject CreatePrimitive(PrimitiveType type)
        {
            return new UnityGameObject(GameObject.CreatePrimitive(type));
        }
    }
}
