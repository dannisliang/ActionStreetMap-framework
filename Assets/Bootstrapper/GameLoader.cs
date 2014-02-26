
using System.IO;
using Mercraft.Maps.Osm;
using Mercraft.Maps.Osm.Data;
using Mercraft.Models;
using Mercraft.Models.Dependencies;
using Mercraft.Models.Map;
using Mercraft.Models.Scene;

namespace Assets.Scripts
{
    /// <summary>
    /// Loads scenas. Component root
    /// </summary>
    public class GameLoader
    {
        private IContainer _container;

        public GameLoader()
        {
            _container = new Container();
        }

        /// <summary>
        /// Loads game
        /// TODO investigate:
        /// 1. settings dependency
        /// 2. reaction on events
        /// </summary>
        public void Load(string file, GeoCoordinate center)
        {
            #region OSM datasource specific types

            // NOTE this should be replaced with real implementation
            // possibly, IDataSourceProvider should be created outside (e.g.
            // read user settings and negotiate data source providers)
            Stream stream = new FileInfo(file).OpenRead();
            _container.RegisterInstance<IDataSourceReadOnly>(MemoryDataSource.CreateFromXmlStream(stream));
            _container.Register(Component.For<IDataSourceProvider>().Use<DefaultDataSourceProvider>().Singleton());
            _container.Register(Component.For<ElementManager>().Use<ElementManager>().Singleton());

            #endregion

            #region Scene

            _container.Register(Component.For<ISceneBuilder>().Use<OsmSceneBuilder>().Singleton());
            _container.RegisterInstance<TileSettings>(new TileSettings()
            {
                RelativeNullPoint = center,
                Size = 1000
            });
            _container.Register(Component.For<TileProvider>().Use<TileProvider>().Singleton());

            #endregion
        }

        public void Unload()
        {
            
        }
    }
}
