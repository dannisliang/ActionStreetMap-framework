using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class ZoneBootstrapper: BootstrapperPlugin
    {
        private const string SceneBuilderKey = "scene";
        private const string TileProviderKey = "provider";
        private const string PositionListenerKey = "loader";
        private const string GameObjectBuilderKey = "gameObjectBuilder";

        public override bool Run()
        {
            Configurator.RegisterComponent<ISceneBuilder>(ConfigSection.GetSection(SceneBuilderKey));
            Configurator.RegisterComponent<TileProvider>(ConfigSection.GetSection(TileProviderKey));
            Configurator.RegisterComponent<IPositionListener>(ConfigSection.GetSection(PositionListenerKey));
            Configurator.RegisterComponent<IGameObjectBuilder>(ConfigSection.GetSection(GameObjectBuilderKey));
            return true;
        }
    }
}
