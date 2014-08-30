using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestCylinderModelBuilder : ModelBuilder
    {
        public override string Name
        {
            get { return "cylinder"; }
        }

        [Dependency]
        public TestCylinderModelBuilder(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, HeightMap heightMap, Rule rule, Area area)
        {
            base.BuildArea(center, heightMap, rule, area);
            return BuildCylinder(center, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, HeightMap heightMap, Rule rule, Way way)
        {
            base.BuildWay(center, heightMap, rule, way);
            return BuildCylinder(center, way.Points, rule);
        }

        private IGameObject BuildCylinder(GeoCoordinate center, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var cylinderCenter = circle.Item2;

            var height = rule.GetHeight();
            var minHeight = rule.GetMinHeight();

            var actualHeight = (height - minHeight)/2;
            return GameObjectFactory.CreatePrimitive("", UnityPrimitiveType.Cylinder);
        }
    }
}