using System;
using System.IO;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;

namespace Mercraft.Explorer.Bootstrappers
{
    public class OsmBootstrapper: BootstrapperPlugin
    {
        public OsmBootstrapper(IConfigSection configSection) : base(configSection)
        {
        }

        public override bool Run()
        {
            var section = ConfigSection.GetSection("dataSourceProvider");
            var dataSourceProviderType = section.GetType("@type");

            Container.Register(Component.For<IDataSourceProvider>().Use(dataSourceProviderType, section));
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
