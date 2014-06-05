using Mercraft.Infrastructure.Config;

namespace Mercraft.Maps.UnitTests
{
    public class TestPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            return path;
        }
    }
}