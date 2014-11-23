using ActionStreetMap.Infrastructure.IO;

namespace ActionStreetMap.Tests
{
    /// <summary>
    ///     Test path provider encapsulates assets and test build path differences.
    /// </summary>
    public class TestPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            // Before, web player was choosen as test platform, we had to give standard extensions to pbf, mapcss, hgt files.
            // However, we didn't  want to have this as requirement to all platforms. So, all differences were encapsulated 
            // in path resolvers

            if (path.StartsWith("test") || path[1] == ':') // absolute path pointing to disk
                return path;

            if (path.StartsWith("Config") || path.StartsWith("Maps"))
                path = "..\\..\\..\\TestAssets\\DemoResources\\" + path;

            //else if (path.Contains("themes") && !path.Contains(@"Assets\Resources"))
            //    path = "..\\..\\..\\TestAssets\\DemoResources\\Config\\themes\\default\\" + path;

            //if (path.EndsWith(".pbf") || path.EndsWith(".hgt"))
            //    path += ".bytes";

            //if (path.EndsWith("mapcss") && !path.Contains("TestAssets"))
            //    path += ".txt";

            return path;
        }
    }
}