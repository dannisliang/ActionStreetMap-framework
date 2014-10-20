using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;

namespace Mercraft.Explorer.Bootstrappers
{
    /// <summary>
    ///     Register OSM-specific classes.
    /// </summary>
    public class OsmBootstrapper: BootstrapperPlugin
    {
        private const string DataSourceProviderKey = "mapdata";

        /// <inheritdoc />
        public override string Name { get { return "osm"; } }

        /// <inheritdoc />
        public override bool Run()
        {
            Container.Register(Component
               .For<IElementSourceProvider>()
               .Use<DefaultElementSourceProvider>()
               .Singleton()
               .SetConfig(GlobalConfigSection.GetSection(DataSourceProviderKey)));

            Container.Register(Component
                .For<ElementManager>()
                .Use<ElementManager>()
                .Singleton());

            return true;
        }
    }
}
