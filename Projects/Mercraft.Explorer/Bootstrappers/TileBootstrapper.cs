using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Core.Elevation.Srtm;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;

namespace Mercraft.Explorer.Bootstrappers
{
    public class TileBootstrapper : BootstrapperPlugin
    {
        private const string TileKey = "tile";
        private const string ElevationKey = "elevationdata";

        public override string Name { get { return "tile"; } }

        public override bool Run()
        {
            Container.Register(Component.For<ITileLoader>().Use<OsmTileLoader>().Singleton());

            Container.Register(Component.For<IHeightMapProvider>().Use<HeightMapProvider>().Singleton());
            Container.Register(Component.For<IElevationProvider>().Use<SrtmElevationProvider>().Singleton()
                .SetConfig(GlobalConfigSection.GetSection(ElevationKey)));
            
            Container.Register(Component
                .For<IPositionListener>()
                .Use<TileManager>()
                .Singleton()
                .SetConfig(GlobalConfigSection.GetSection(TileKey)));
            
            return true;
        }
    }
}