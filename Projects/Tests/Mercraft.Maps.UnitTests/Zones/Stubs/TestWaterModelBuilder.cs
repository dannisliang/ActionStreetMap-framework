using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Scene;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    class TestWaterModelBuilder : IModelBuilder
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public string Name
        {
            get { return "water"; }
        }

        public IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            return _gameObjectFactory.CreateNew(area.ToString());
        }

        public IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            return _gameObjectFactory.CreateNew(way.ToString());
        }

        public IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            return _gameObjectFactory.CreateNew(node.ToString());
        }

        [Dependency]
        public TestWaterModelBuilder(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }
    }
}
