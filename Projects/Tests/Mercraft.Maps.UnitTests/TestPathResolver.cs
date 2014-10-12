using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.IO;

namespace Mercraft.Maps.UnitTests
{
    public class TestPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            if (path.StartsWith(".") || path.StartsWith("test") || path.StartsWith("f:"))
                return path;

            if (path.StartsWith("Config") || path.StartsWith("Maps") || path.StartsWith("Elevation"))
                return "..\\..\\..\\..\\..\\Demo\\" + path;

            return "..\\..\\..\\..\\..\\Demo\\Config\\themes\\default\\" + path;
        }
    }
}