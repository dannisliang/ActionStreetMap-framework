using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;

namespace Mercraft.Explorer.Bootstrappers
{
    public class ZoneBootstrapper: BootstrapperPlugin
    {
        public override bool Run()
        {
            //var zoneXPath = ConfigSection.GetString("@path");
            //var zoneConfig = Config.GetSection(zoneXPath);

            Configurator.RegisterComponent<ISceneBuilder>(ConfigSection.GetSection("scene"));
            Configurator.RegisterComponent<TileProvider>(ConfigSection.GetSection("provider"));
            Configurator.RegisterComponent<ITerrainBuilder>(ConfigSection.GetSection("terrain"));
            Configurator.RegisterComponent<IPositionListener>(ConfigSection.GetSection("loader"));

            // register builders
            foreach (var builderConfig in ConfigSection.GetSections("builders/builder"))
            {
                Configurator.RegisterNamedComponent<ISceneModelVisitor>(builderConfig);
            }

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
