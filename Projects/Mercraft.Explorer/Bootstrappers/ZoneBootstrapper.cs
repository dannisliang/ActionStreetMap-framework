using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class ZoneBootstrapper : BootstrapperPlugin
    {
        private const string SceneBuilderKey = "scene";
        private const string TileProviderKey = "provider";
        private const string PositionListenerKey = "loader";
        private const string TileListenerKey = "listeners/tile";
        private const string ZoneListenerKey = "listeners/zone";

        public override bool Run()
        {
            Configurator.RegisterComponent<ISceneBuilder>(ConfigSection.GetSection(SceneBuilderKey));
            Configurator.RegisterComponent<TileProvider>(ConfigSection.GetSection(TileProviderKey));
            Configurator.RegisterComponent<IPositionListener>(ConfigSection.GetSection(PositionListenerKey));
            Configurator.RegisterComponent<ITileListener>(ConfigSection.GetSection(TileListenerKey));
            Configurator.RegisterComponent<IZoneListener>(ConfigSection.GetSection(ZoneListenerKey));

            return true;
        }
    }
}