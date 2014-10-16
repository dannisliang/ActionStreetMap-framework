using Mercraft.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    public class DemoPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            // WEB
            //if (path.EndsWith(".hgt") || path.EndsWith(".pbf") || path.EndsWith(".mapcss"))
           //     return path.Replace(@"\", @"/");
           // return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace(@"\",@"/");
            
            // WINDOWS
            if (path.EndsWith(".mapcss") || path.EndsWith(".json"))
                path = "Assets//Resources//" + path;

            if (path.EndsWith(".mapcss"))
                path += ".txt";

            return path;
            
            // ANDROID
            //return String.Format("/sdcard/Mercraft/{0}", path.Replace(@"\", "/"));
        }
    }
}