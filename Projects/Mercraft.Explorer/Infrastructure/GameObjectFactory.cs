using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Explorer.Infrastructure
{
    public class GameObjectFactory : IGameObjectFactory
    {
        public virtual IGameObject CreateNew(string name)
        {
            return new UnityGameObject(name);
        }

        public virtual IGameObject CreatePrimitive(string name, PrimitiveType type)
        {
            return new UnityGameObject(name, GameObject.CreatePrimitive(type));
        }
    }
}
