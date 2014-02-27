using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Bootstrap;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SettingsBootstrapper: BootstrapperPlugin
    {
        private readonly TileSettings _tileSettings = new TileSettings()
        {
            RelativeNullPoint = new GeoCoordinate(52.529814, 13.388015),
            Size = 1000
        };

        public SettingsBootstrapper(): base("Bootstrappers.Settings")
        {
        }

        public override bool Load()
        {
            // NOTE: external lifecycle manager is used in case of RegisterInstance
            // so, we need to hold reference for these object to prevent GC
            Container.RegisterInstance(_tileSettings, "Settings.Tile");
            Container.RegisterInstance(_tileSettings.RelativeNullPoint, "Settings.GeoCenter");
            return true;
        }

        public override bool Update()
        {
            return true;
        }

        public override bool Unload()
        {
            return true;
        }
    }
}
