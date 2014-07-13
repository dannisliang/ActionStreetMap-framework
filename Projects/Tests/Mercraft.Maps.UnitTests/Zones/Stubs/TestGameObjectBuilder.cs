using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Interactions;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;

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

        private List<RoadSettings> _roads = new List<RoadSettings>();

        [Dependency]
        public TestGameObjectBuilder(IGameObjectFactory goFactory,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _goFactory = goFactory;
            _builders = builders.ToList();
            _behaviours = behaviours.ToList();
        }

        #region IGameObjectBuilder implementation

        public IGameObject CreateTileHolder()
        {
            return _goFactory.CreateNew("tile");
        }

        public IGameObject FromCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas)
        {
            var tile = canvas.Tile;
            //var material = rule.GetMaterial();
            return _goFactory.CreatePrimitive("", UnityPrimitiveType.Quad);
        }

        public IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            if (rule.IsSkipped())
                return _goFactory.CreateNew(String.Format("skip {0}", area));

            if (rule.IsTerrain())
            {
                return null;
            }

            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildArea(center, rule, area);
            ApplyBehaviour(gameObjectWrapper, rule, area);

            return gameObjectWrapper;
        }

        public IGameObject FromWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way)
        {
            if (rule.IsSkipped())
                return _goFactory.CreateNew(String.Format("skip {0}", way));

            if (rule.IsRoad())
            {
                var roadGameObject = _goFactory.CreateNew(way.ToString());
                _roads.Add(new RoadSettings()
                {
                    Width = (int)Math.Round(rule.GetWidth()),
                    TargetObject = roadGameObject,
                    Points = way.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray()
                });
                // this game object should be initialized inside of TerrainBuilder's logic
                return roadGameObject;
            }

            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildWay(center, rule, way);
            ApplyBehaviour(gameObjectWrapper, rule, way);

            return gameObjectWrapper;
        }

        #endregion

        private void ApplyBehaviour(IGameObject target, Rule rule, Model model)
        {
            var behaviour = rule.GetModelBehaviour(_behaviours);
            if (behaviour != null)
                behaviour.Apply(target, model);
        }
    }
}