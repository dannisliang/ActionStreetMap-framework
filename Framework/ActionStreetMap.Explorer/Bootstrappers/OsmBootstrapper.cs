using ActionStreetMap.Infrastructure.Bootstrap;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Maps.Osm;
using ActionStreetMap.Maps.Osm.Data;

namespace ActionStreetMap.Explorer.Bootstrappers
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
