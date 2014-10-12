
using System.IO;

namespace Mercraft.Infrastructure.IO
{
    public class FileSystemService: IFileSystemService
    {
        private readonly IPathResolver _pathResolver;

        public FileSystemService(IPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        public Stream ReadStream(string path)
        {
            return File.Open(_pathResolver.Resolve(path), FileMode.Open);
        }

        public string ReadText(string path)
        {
            using (var reader = new StreamReader(_pathResolver.Resolve(path)))
            {
                return reader.ReadToEnd();
            }
        }

        public byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(_pathResolver.Resolve(path));
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetDirectories(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }
    }
}
