using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;

namespace Mercraft.Explorer.Scene.Builders
{
    public class DetailModelBuilder: ModelBuilder
    {
        public override string Name
        {
            get { return "detail"; }
        }

        public DetailModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory) :
            base(worldManager, gameObjectFactory)
        {
        }

        public override IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            // TODO
            return null;
        }
    }
}
