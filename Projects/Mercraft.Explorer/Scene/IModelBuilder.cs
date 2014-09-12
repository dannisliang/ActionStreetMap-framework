using System;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

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
        protected ITrace Trace { get; set; }

        protected readonly IGameObjectFactory GameObjectFactory;
        protected readonly WorldManager WorldManager;

        [Dependency]
        protected ModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory)
        {
            WorldManager = worldManager;
            GameObjectFactory = gameObjectFactory;
        }

        public virtual IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            Trace.Normal(String.Format("{0}: building area {1}", Name, area.Id));
            return null;
        }

        public virtual IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            Trace.Normal(String.Format("{0}: building way {1}", Name, way.Id));
            return null;
        }

        public virtual IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            Trace.Normal(String.Format("{0}: building node {1}", Name, node.Id));
            return null;
        }
    }
}
