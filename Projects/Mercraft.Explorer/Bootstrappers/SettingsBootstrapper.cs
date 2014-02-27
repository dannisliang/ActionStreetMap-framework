using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SettingsBootstrapper: BootstrapperPlugin
    {
        private TileSettings _tileSettings;

        public SettingsBootstrapper(IConfigSection configSection) : base(configSection)
        {
        }

        public override bool Run()
        {
            var tileSize = ConfigSection.GetFloat("tile/@size");
            _tileSettings = new TileSettings()
            {
                Size = tileSize
            };

            // NOTE: external lifecycle manager is used in case of RegisterInstance
            // so, we need to hold reference for these object to prevent GC
            Container.RegisterInstance(_tileSettings, "Settings.Tile");
            
            return true;
        }

        public override bool Update()
        {
            return true;
        }

        public override bool Stop()
        {
            return true;
        }
    }
}
