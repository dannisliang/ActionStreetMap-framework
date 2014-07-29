using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Builders;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestDefaultModelBuilder : ModelBuilder
    {
        [Dependency]
        public TestDefaultModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew("");
            BuildModel(center, gameObjectWrapper, rule, area.Points.ToList());
            return gameObjectWrapper;
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew("");
            BuildModel(center, gameObjectWrapper, rule, way.Points.ToList());

            return gameObjectWrapper;
        }

        private void BuildModel(GeoCoordinate center, IGameObject gameObject, Rule rule,
            IList<GeoCoordinate> coordinates)
        {
        }
    }
}