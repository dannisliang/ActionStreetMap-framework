using System;
using System.IO;
using Mercraft.Core;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.IO;

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
        IElementSource Get(BoundingBox bbox);
    }

    /// <summary>
    /// Default implementation of IElementSourceProvider
    /// to return different dataSources by geo coordinates
    /// </summary>
    public class DefaultElementSourceProvider : IElementSourceProvider, IConfigurable
    {
        private IElementSource _dataSource;
        private readonly IFileSystemService _fileSystemService;

        [Dependency]
        public DefaultElementSourceProvider(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public IElementSource Get(BoundingBox bbox)
        {
            return _dataSource;
        }

        public void Configure(IConfigSection configSection)
        {
            string filePath = configSection.GetString("");
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

            if (String.IsNullOrEmpty(fileExtension))
            {
                _dataSource = new PbfIndexListElementSource(filePath, _fileSystemService);
            }
            else
            {
                // TODO dispose opened stream
                Stream stream = _fileSystemService.ReadStream(filePath);
                _dataSource = fileExtension == ".xml"
                    ? (IElementSource) new XmlElementSource(stream)
                    : (IElementSource) new PbfElementSource(stream);
            }
        }
    }

}
