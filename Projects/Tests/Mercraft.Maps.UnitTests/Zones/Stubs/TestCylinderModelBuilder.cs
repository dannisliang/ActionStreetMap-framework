using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Scene;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestCylinderModelBuilder : ModelBuilder
    {
        public override string Name
        {
            get { return "cylinder"; }
        }

        [Dependency]
        public TestCylinderModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory) :
            base(worldManager, gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);
            return BuildCylinder(tile.RelativeNullPoint, area.Points, rule);
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            base.BuildWay(tile, rule, way);
            return BuildCylinder(tile.RelativeNullPoint, way.Points, rule);
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