using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Interactions;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    /// <summary>
    ///     Emulates real GameObjectBuilder without unity specific classes which throw SecurityException
    /// </summary>
    public class TestGameObjectBuilder : IGameObjectBuilder
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        [Dependency]
        public TestGameObjectBuilder(IGameObjectFactory goFactory,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _goFactory = goFactory;
            _builders = builders;
            _behaviours = behaviours;
        }

        #region IGameObjectBuilder implementation

        public IGameObject FromCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas)
        {
            var tile = canvas.Tile;
            //var material = rule.GetMaterial();
            return _goFactory.CreatePrimitive("", UnityPrimitiveType.Quad);
        }

        public IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildArea(center, rule, area);
            ApplyBehaviour(gameObjectWrapper, rule);

            return gameObjectWrapper;
        }

        public IGameObject FromWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way)
        {
            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildWay(center, rule, way);
            ApplyBehaviour(gameObjectWrapper, rule);

            return gameObjectWrapper;
        }

        #endregion

        private void ApplyBehaviour(IGameObject target, Rule rule)
        {
            var behaviour = rule.GetModelBehaviour(_behaviours);
            if (behaviour != null)
                behaviour.Apply(target);
        }
    }
}