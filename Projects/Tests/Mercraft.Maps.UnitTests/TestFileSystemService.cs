using System.IO;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.IO;

namespace ActionStreetMap.Maps.UnitTests
{
    public class TestFileSystemService: IFileSystemService
    {
        private readonly IFileSystemService _impl;

        [Dependency]
        public TestFileSystemService(IPathResolver pathResolver)
        {
            _impl = new FileSystemService(pathResolver);
        }
        public Stream ReadStream(string path)
        {
            return _impl.ReadStream(path);
        }

        public string ReadText(string path)
        {
            return _impl.ReadText(path);
        }

        public byte[] ReadBytes(string path)
        {
            return _impl.ReadBytes(path);
        }

        public bool Exists(string path)
        {
            return _impl.Exists(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            if (path[1] != ':' && searchPattern == "*.list")
                return _impl.GetFiles(path, searchPattern + "*.txt");
           
            return _impl.GetFiles(path, searchPattern);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return _impl.GetDirectories(path, searchPattern);
        }
    }
}
