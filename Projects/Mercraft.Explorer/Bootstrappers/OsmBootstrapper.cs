using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;

namespace Mercraft.Explorer.Bootstrappers
{
    public class OsmBootstrapper: BootstrapperPlugin
    {
        private const string DataSourceProviderKey = "dataSourceProvider";

        public override bool Run()
        {
            Configurator.RegisterComponent<IElementSourceProvider>(
                ConfigSection.GetSection(DataSourceProviderKey));

            Container.Register(Component.For<ElementManager>().Use<ElementManager>().Singleton());

            return true;
        }
    }
}
