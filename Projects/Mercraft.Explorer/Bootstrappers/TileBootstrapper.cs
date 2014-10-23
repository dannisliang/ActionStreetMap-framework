using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Core.Elevation.Srtm;
using Mercraft.Core.Scene;
using Mercraft.Explorer.Scene;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;

namespace Mercraft.Explorer.Bootstrappers
{
    /// <summary>
    ///     Register tile processing classes.
    /// </summary>
    public class TileBootstrapper : BootstrapperPlugin
    {
        private const string TileKey = "tile";
        private const string ElevationKey = "elevationdata";

        /// <inheritdoc />
        public override string Name { get { return "tile"; } }

        /// <inheritdoc />
        public override bool Run()
        {
            Container.Register(Component.For<ITileLoader>().Use<OsmTileLoader>().Singleton());

            // activates/deactivates tiles during the game based on distance to player
            Container.Register(Component.For<ITileActivator>().Use<TileActivator>().Singleton());

            Container.Register(Component
                .For<IHeightMapProvider>()
                .Use<HeightMapProvider>()
                .Singleton()
                .SetConfig(GlobalConfigSection.GetSection(TileKey)));

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