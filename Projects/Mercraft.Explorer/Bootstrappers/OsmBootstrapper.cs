using System.IO;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;

namespace Mercraft.Explorer.Bootstrappers
{
    public class OsmBootstrapper: BootstrapperPlugin
    {
        private const string OsmFile = @".\Projects\Tests\TestAssets\berlin_house.osm.xml";

        public OsmBootstrapper(): base("Bootstrappers.Osm")
        {
        }

        public override bool Load()
        {
            Stream stream = new FileInfo(OsmFile).OpenRead();
            Container.RegisterInstance<IDataSourceReadOnly>(MemoryDataSource.CreateFromXmlStream(stream));
            Container.Register(Component.For<IDataSourceProvider>().Use<DefaultDataSourceProvider>().Singleton());
            Container.Register(Component.For<ElementManager>().Use<ElementManager>().Singleton());

            return true;
        }

        public override bool Update()
        {
            return true;
        }

        public override bool Unload()
        {
            return true;
        }
    }
}
