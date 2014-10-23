using System;
using System.IO;
using Mercraft.Core;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Infrastructure.IO;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    ///     Provides the way to get OSM datasource by geocoordinate.
    /// </summary>
    public interface IElementSourceProvider
    {
        /// <summary>
        ///     Returns OSM datasource by bounding box..
        /// </summary>
        IElementSource Get(BoundingBox bbox);
    }

    /// <summary>
    ///     Default implementation of IElementSourceProvider to return different dataSources depends on configuration.
    /// </summary>
    public class DefaultElementSourceProvider : IElementSourceProvider, IConfigurable, IDisposable
    {
        private IElementSource _dataSource;
        private readonly IFileSystemService _fileSystemService;
        private readonly ITrace _trace;

        /// <summary>
        ///     Creates DefaultElementSourceProvider
        /// </summary>
        /// <param name="fileSystemService">File system service.</param>
        /// <param name="trace">Trace.</param>
        [Dependency]
        public DefaultElementSourceProvider(IFileSystemService fileSystemService, ITrace trace)
        {
            _fileSystemService = fileSystemService;
            _trace = trace;
        }

        /// <inheritdoc />
        public IElementSource Get(BoundingBox bbox)
        {
            return _dataSource;
        }

        /// <inheritdoc />
        public void Configure(IConfigSection configSection)
        {
            string filePath = configSection.GetString("");
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

            if (String.IsNullOrEmpty(fileExtension))
            {
                _dataSource = new PbfIndexListElementSource(filePath, _fileSystemService, _trace);
            }
            else
            {
                // TODO dispose opened stream
                Stream stream = _fileSystemService.ReadStream(filePath);
                _dataSource = fileExtension == ".xml"
                    ? new XmlElementSource(stream)
                    : (IElementSource) new PbfElementSource(stream);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataSource.Dispose();
            }
        }
    }
}