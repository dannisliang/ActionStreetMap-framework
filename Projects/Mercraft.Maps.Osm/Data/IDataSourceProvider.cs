using System.IO;
using Mercraft.Infrastructure.Config;
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
    public class DefaultDataSourceProvider : IDataSourceProvider, IConfigurable
    {
        private IDataSourceReadOnly _dataSource;

        public IDataSourceReadOnly Get(GeoCoordinate coordinate)
        {
            return _dataSource;
        }

        public void Configure(IConfigSection configSection)
        {
            string filePath = configSection.GetString("file");
            bool isXml = configSection.GetBool("file/@xml");
            Stream stream = new FileInfo(filePath).OpenRead();
            _dataSource = isXml
                ? MemoryDataSource.CreateFromXmlStream(stream)
                : MemoryDataSource.CreateFromPbfStream(stream);
        }
    }
}
