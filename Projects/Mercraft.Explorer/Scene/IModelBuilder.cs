using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Infrastructure.Utilities;

namespace Mercraft.Explorer.Scene
{
    /// <summary>
    ///     Defines model builder logic.
    /// </summary>
    public interface IModelBuilder
    {
        /// <summary>
        ///     Name of model builder.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Builds model from area.
        /// </summary>
        /// <param name="tile">Tile.</param>
        /// <param name="rule">Rule.</param>
        /// <param name="area">Area.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject BuildArea(Tile tile, Rule rule, Area area);

        /// <summary>
        ///     Builds model from way.
        /// </summary>
        /// <param name="tile">Tile.</param>
        /// <param name="rule">Rule.</param>
        /// <param name="way">Way.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject BuildWay(Tile tile, Rule rule, Way way);

        /// <summary>
        ///     Builds model from node.
        /// </summary>
        /// <param name="tile">Tile.</param>
        /// <param name="rule">Rule.</param>
        /// <param name="node">Node.</param>
        /// <returns>Game object wrapper.</returns>
        IGameObject BuildNode(Tile tile, Rule rule, Node node);
    }

    /// <summary>
    ///     Defines base class for model builders which provides helper logic.
    /// </summary>
    public abstract class ModelBuilder : IModelBuilder
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <summary>
        ///     Trace.
        /// </summary>
        [Dependency]
        public ITrace Trace { get; set; }

        /// <summary>
        ///     World manager.
        /// </summary>
        protected readonly WorldManager WorldManager;

        /// <summary>
        ///     Game object factory.
        /// </summary>
        protected readonly IGameObjectFactory GameObjectFactory;

        /// <summary>
        ///     Object pool.
        /// </summary>
        protected readonly IObjectPool ObjectPool;

        /// <summary>
        ///     Creates ModelBuilder.
        /// </summary>
        /// <param name="worldManager">World manager.</param>
        /// <param name="gameObjectFactory">Game object factory.</param>
        /// <param name="objectPool">Object pool.</param>
        protected ModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, IObjectPool objectPool)
        {
            WorldManager = worldManager;
            GameObjectFactory = gameObjectFactory;
            ObjectPool = objectPool;
        }

        /// <inheritdoc />
        public virtual IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            //Trace.Normal(String.Format("{0}: building area {1}", Name, area.Id));
            return null;
        }

        /// <inheritdoc />
        public virtual IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            //Trace.Normal(String.Format("{0}: building way {1}", Name, way.Id));
            return null;
        }

        /// <inheritdoc />
        public virtual IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            //Trace.Normal(String.Format("{0}: building node {1}", Name, node.Id));
            return null;
        }
    }
}
