using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Core.Elevation.Srtm;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;

namespace Mercraft.Explorer.Bootstrappers
{
    public class ZoneBootstrapper : BootstrapperPlugin
    {
        private const string TileKey = "tile";
        private const string PositionKey = "position";
        private const string ElevationKey = "elevationdata";

        public override string Name { get { return "zone"; } }

        public override bool Run()
        {
            Container.Register(Component.For<ISceneBuilder>().Use<OsmSceneBuilder>().Singleton());

            Container.Register(Component.For<IHeightMapProvider>().Use<HeightMapProvider>().Singleton());
            Container.Register(Component.For<IElevationProvider>().Use<SrtmElevationProvider>().Singleton()
                .SetConfig(GlobalConfigSection.GetSection(ElevationKey)));
            
            Container.Register(Component
                .For<TileProvider>()
                .Use<TileProvider>()
                .Singleton()
                .SetConfig(GlobalConfigSection.GetSection(TileKey)));


            Container.Register(Component
                .For<IPositionListener>()
                .Use<ZoneLoader>()
                .Singleton()
                .SetConfig(GlobalConfigSection.GetSection(PositionKey)));
            
            return true;
        }
    }
}