using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm;
using Mercraft.Models.Scene;
using Mercraft.Models.Tiles;

namespace Assets.Bootstrappers
{
    public class SceneBootstrapper: BootstrapperPlugin
    {
        public SceneBootstrapper(): base("Bootstrappers.Scene")
        {
        }

        public override bool Load()
        {
            UnityEngine.Debug.Log("Scene");
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
