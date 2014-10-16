using Mercraft.Infrastructure.IO;

namespace Mercraft.Maps.UnitTests
{
    /// <summary>
    ///  Test path provider encapsulates assets and test build path differences. As web player is choosen
    ///  as test platform, we have to give standard extensions to pbf, mapcss, hgt files. However, we don't 
    ///  want to have this as requirement to all platforms. So, all differences are encapsulated in path resolvers
    /// </summary>
    public class TestPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            if (path.StartsWith("test") || path[1] == ':') // absolute path pointing to disk
                return path;

            if (path.StartsWith("Config") || path.StartsWith("Maps") || path.StartsWith("Elevation"))
                path = "..\\..\\..\\..\\..\\Demo\\Assets\\Resources\\" + path;

            else if (path.Contains("themes") && !path.Contains(@"Assets\Resources"))
                path = "..\\..\\..\\..\\..\\Demo\\Assets\\Resources\\Config\\themes\\default\\" + path;

            if (path.EndsWith(".pbf") || path.EndsWith(".hgt"))
                path += ".bytes";

            if (path.EndsWith("mapcss") && !path.Contains("TestAssets"))
                path += ".txt";

            return path;
        }
    }
}