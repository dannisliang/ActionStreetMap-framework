using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;

namespace Mercraft.Explorer.Bootstrappers
{
    public class OsmBootstrapper: BootstrapperPlugin
    {
        public override bool Run()
        {
            var dataSourceProviderSection = ConfigSection.GetSection("dataSourceProvider");
            Configurator.RegisterComponent<IDataSourceProvider>(dataSourceProviderSection);

            Container.Register(Component.For<ElementManager>().Use<ElementManager>().Singleton());

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
