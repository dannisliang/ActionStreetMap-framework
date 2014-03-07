using System.IO;
using Mercraft.Core;
using Mercraft.Infrastructure.Config;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Provides the way to get OSM datasource by geocoordinate
    /// </summary>
    public interface IElementSourceProvider
    {
        /// <summary>
        /// Returns OSM datasource by geocoordinate
        /// </summary>
        IElementSource Get(GeoCoordinate coordinate);
    }

    /// <summary>
    /// Trivial implementation of IElementSourceProvider
    /// TODO: for development purpose only - real implementation should be able 
    /// to return different dataSources by geo coordinates
    /// </summary>
    public class DefaultElementSourceProvider : IElementSourceProvider, IConfigurable
    {
        private IElementSource _dataSource;

        public IElementSource Get(GeoCoordinate coordinate)
        {
            return _dataSource;
        }

        public void Configure(IConfigSection configSection)
        {
            string filePath = configSection.GetString("file");
            bool isXml = configSection.GetBool("file/@xml");
            Stream stream = new FileInfo(filePath).OpenRead();
            _dataSource = isXml
                ? (IElementSource)new XmlElementSource(stream)
                : (IElementSource)new PbfElementSource(stream);
        }
    }

}
