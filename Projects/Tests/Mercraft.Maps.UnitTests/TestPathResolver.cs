using Mercraft.Infrastructure.Config;

namespace Mercraft.Maps.UnitTests
{
    public class TestPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            if (path.StartsWith(".") || path.StartsWith("test"))
                return path;

            if (path.StartsWith("Config") || path.StartsWith("Maps"))
                return "..\\..\\..\\..\\..\\Demo\\" + path;

            return "..\\..\\..\\..\\..\\Demo\\Config\\themes\\default\\" + path;
        }
    }
}