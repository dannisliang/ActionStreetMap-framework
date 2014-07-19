using System.Collections.Generic;
using System.Reflection;
using Mercraft.Core.Scene;
using Mercraft.Core.Unity;
using Mercraft.Explorer;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestGameObjectFactory : IGameObjectFactory
    {
        public IGameObject CreateNew(string name)
        {
            return new TestGameObject();
        }

        public IGameObject CreateNew(string name, IGameObject parent)
        {
            var go = CreateNew(name);
            go.Parent = parent;
            return go;
        }

        public IGameObject CreatePrimitive(string name, UnityPrimitiveType type)
        {
            return new TestGameObject();
        }

        public ISceneVisitor CreateVisitor(IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            var sceneVisitor = new SceneVisitor(this, builders, behaviours);
            // NOTE this is workaround: I don't want to create interface so far
            typeof(SceneVisitor)
               .GetField("_terrainBuilder", BindingFlags.Instance | BindingFlags.NonPublic)
               .SetValue(sceneVisitor, new TestableTerrainBuilder());

            return sceneVisitor;
        }

        private class TestableTerrainBuilder: TerrainBuilder
        {
            public override IGameObject Build(IGameObject parent, TerrainSettings settings)
            {
                return null;
            }
        }
    }
}