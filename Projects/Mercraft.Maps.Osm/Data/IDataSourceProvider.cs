using System.IO;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Core;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Provides the way to get OSM datasource by geocoordinate
    /// </summary>
    public interface IDataSourceProvider
    {
        /// <summary>
        /// Returns OSM datasource by geocoordinate
        /// </summary>
        IDataSourceReadOnly Get(GeoCoordinate coordinate);
    }

    /// <summary>
    /// Trivial implementation of IDataSourceProvider
    /// TODO: for development purpose only - real implementation should be able 
    /// to return different dataSources by geo coordinates
    /// </summary>
    public class DefaultDataSourceProvider : IDataSourceProvider
    {
        private readonly IDataSourceReadOnly _dataSource;

        [Dependency("")]
        public DefaultDataSourceProvider(IConfigSection config)
        {
            string filePath = config.GetString("path");
            bool isXml = config.GetBool("path/@xml");
            Stream stream = new FileInfo(filePath).OpenRead();
            _dataSource = isXml
                ? MemoryDataSource.CreateFromXmlStream(stream)
                : MemoryDataSource.CreateFromPbfStream(stream);
        }

        public IDataSourceReadOnly Get(GeoCoordinate coordinate)
        {
            return _dataSource;
        }
    }
}
