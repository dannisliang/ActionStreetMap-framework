using System.IO;

namespace Mercraft.Infrastructure.IO
{
    public interface IFileSystemService
    {
        Stream ReadStream(string path);
        string ReadText(string path);
        byte[] ReadBytes(string path);

        bool Exists(string path);
        string[] GetFiles(string path, string searchPattern);
        string[] GetDirectories(string path, string searchPattern);
    }
}
