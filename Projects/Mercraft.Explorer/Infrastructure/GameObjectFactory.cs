using System.Collections.Generic;
using Mercraft.Core.Scene;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Infrastructure
{
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly IThemeProvider _themeProvider;
        private readonly ITerrainBuilder _terrainBuilder;

        [Dependency]
        public GameObjectFactory(IThemeProvider themeProvider, ITerrainBuilder terrainBuilder)
        {
            _themeProvider = themeProvider;
            _terrainBuilder = terrainBuilder;
        }

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
