using System;
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

        public IGameObject CreateNew(string name, IGameObject parent)
        {
            var go = CreateNew(name);
            go.Parent = parent;
            return go;
        }

        public virtual IGameObject CreatePrimitive(string name, UnityPrimitiveType type)
        {
            return new UnityGameObject(name, GetPrimitive(type));
        }

        public IGameObject Wrap(string name, object gameObject)
        {
            var instance = gameObject as GameObject;
            if(instance == null)
                throw new ArgumentException(
                    String.Format("Unable to wrap {0}. Expecting UnityEngine.GameObject", gameObject), "gameObject");

            return new UnityGameObject(name, instance);
        }

        private GameObject GetPrimitive(UnityPrimitiveType type)
        {
            switch (type)
            {
                case UnityPrimitiveType.Capsule:
                    return GameObject.CreatePrimitive(PrimitiveType.Capsule);
                case UnityPrimitiveType.Cube:
                    return GameObject.CreatePrimitive(PrimitiveType.Cube);
                case UnityPrimitiveType.Cylinder:
                    return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                case UnityPrimitiveType.Plane:
                    return GameObject.CreatePrimitive(PrimitiveType.Plane);
                case UnityPrimitiveType.Quad:
                    return GameObject.CreatePrimitive(PrimitiveType.Quad);
                case UnityPrimitiveType.Sphere:
                    return GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            return null;
        }
    }
}
