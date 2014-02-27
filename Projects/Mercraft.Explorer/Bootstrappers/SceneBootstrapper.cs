using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;

namespace Mercraft.Explorer.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        public SceneBootstrapper(IConfigSection configSection) : base(configSection)
        {
        }

        public override bool Run()
        {
            Container.Register(Component.For<ISceneBuilder>().Use<OsmSceneBuilder>().Singleton());
            Container.Register(Component.For<TileProvider>().Use<TileProvider>().Singleton());
            Container.Register(Component.For<IPositionListener>().Use<ZoneLoader>().Singleton());

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
