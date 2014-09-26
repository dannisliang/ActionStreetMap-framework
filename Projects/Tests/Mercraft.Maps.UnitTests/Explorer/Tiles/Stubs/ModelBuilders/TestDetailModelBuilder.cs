using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Scene.Builders;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs.ModelBuilders
{
    class TestDetailModelBuilder: DetailModelBuilder
    {
        [Dependency]
        public TestDetailModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, 
            IResourceProvider resourceProvider) : base(worldManager, gameObjectFactory, resourceProvider)
        {
        }

        protected override IGameObject BuildObject(Tile tile, Rule rule, Node node, MapPoint mapPoint, float zIndex, string detail)
        {
            // do nothing
            return null;
        }
    }
}
