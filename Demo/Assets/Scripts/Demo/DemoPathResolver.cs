using Mercraft.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    public class DemoPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {           
            // WINDOWS
            if (path.EndsWith(".mapcss") || path.EndsWith(".json"))
                path = "Assets//Resources//" + path;

            if (path.EndsWith(".mapcss"))
                path += ".txt";

            return path;
        }
    }
}