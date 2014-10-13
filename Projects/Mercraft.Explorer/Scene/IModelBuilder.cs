using System;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Infrastructure.Utilities;

namespace Mercraft.Explorer.Scene
{
    public interface IModelBuilder
    {
        string Name { get; }
        IGameObject BuildArea(Tile tile, Rule rule, Area area);
        IGameObject BuildWay(Tile tile, Rule rule, Way way);
        IGameObject BuildNode(Tile tile, Rule rule, Node node);
    }

    public abstract class ModelBuilder : IModelBuilder
    {
        public abstract string Name { get; }

        [Dependency]
        public ITrace Trace { get; set; }

        protected readonly WorldManager WorldManager;
        protected readonly IGameObjectFactory GameObjectFactory;
        protected readonly IObjectPool ObjectPool;

        protected ModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, IObjectPool objectPool)
        {
            WorldManager = worldManager;
            GameObjectFactory = gameObjectFactory;
            ObjectPool = objectPool;
        }

        public virtual IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            //Trace.Normal(String.Format("{0}: building area {1}", Name, area.Id));
            return null;
        }

        public virtual IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            //Trace.Normal(String.Format("{0}: building way {1}", Name, way.Id));
            return null;
        }

        public virtual IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            //Trace.Normal(String.Format("{0}: building node {1}", Name, node.Id));
            return null;
        }
    }
}
