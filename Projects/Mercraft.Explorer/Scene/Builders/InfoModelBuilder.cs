using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Explorer.Scene.Builders
{
    public class InfoModelBuilder: ModelBuilder
    {
        public override string Name
        {
            get { return "info"; }
        }

        [Dependency]
        public InfoModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory) :
            base(worldManager, gameObjectFactory)
        {
        }

        public override IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {

            return null;
        }
    }
}
