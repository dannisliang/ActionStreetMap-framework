using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        public SceneBootstrapper(): base("Bootstrappers.Scene")
        {
        }

        public override bool Load()
        {
            Container.Register(Component.For<ISceneBuilder>().Use<OsmSceneBuilder>().Singleton());
            Container.Register(Component.For<TileProvider>().Use<TileProvider>().Singleton());

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
