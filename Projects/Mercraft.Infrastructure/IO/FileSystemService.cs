using System.IO;

namespace Mercraft.Infrastructure.IO
{
    /// <summary>
    ///     Provides a way to interact with regular file system.
    /// </summary>
    public class FileSystemService: IFileSystemService
    {
        private readonly IPathResolver _pathResolver;

        /// <summary>
        ///     Creates FileSystemService
        /// </summary>
        /// <param name="pathResolver"></param>
        public FileSystemService(IPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        /// <inheritdoc />
        public Stream ReadStream(string path)
        {
            return File.Open(_pathResolver.Resolve(path), FileMode.Open);
        }

        /// <inheritdoc />
        public string ReadText(string path)
        {
            using (var reader = new StreamReader(_pathResolver.Resolve(path)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <inheritdoc />
        public byte[] ReadBytes(string path)
        {
#if SANDBOX
            throw new System.NotSupportedException("This code cannot be used with defined build symbols");
#else
            return File.ReadAllBytes(_pathResolver.Resolve(path));
#endif
        }

        /// <inheritdoc />
        public bool Exists(string path)
        {
            return File.Exists(_pathResolver.Resolve(path));
        }

        /// <inheritdoc />
        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(_pathResolver.Resolve(path), searchPattern);
        }

        /// <inheritdoc />
        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(_pathResolver.Resolve(path), searchPattern);
        }
    }
}
