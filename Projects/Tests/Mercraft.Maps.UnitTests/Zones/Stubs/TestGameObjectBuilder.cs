using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestGameObjectBuilder : IGameObjectBuilder
    {
        public IGameObject FromCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas)
        {
            return new TestGameObject();
        }

        public IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            return new TestGameObject();
        }

        public IGameObject FromWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way)
        {
            return new TestGameObject();
        }
    }
}
