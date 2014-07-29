using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestBuildingModelBuilder : ModelBuilder
    {

        private string _theme = "berlin";

        [Dependency]
        public TestBuildingModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        private const int NoValue = 0;

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildBuilding(center, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            return BuildBuilding(center, way, way.Points, rule);
        }

        private IGameObject BuildBuilding(GeoCoordinate center, Model model, GeoCoordinate[] footPrint, Rule rule)
        {
            var gameObjectWrapper = GameObjectFactory.CreateNew("");

            var verticies = PolygonHelper.GetVerticies2D(center, footPrint);
            var height = rule.GetHeight(NoValue);
            var levels = rule.GetLevels(NoValue);

            var styleName = rule.GetBuildingType();


            return gameObjectWrapper;
        }
    }
}