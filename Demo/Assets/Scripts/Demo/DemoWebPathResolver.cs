using System.IO;
using Mercraft.Infrastructure.IO;

namespace Assets.Scripts.Demo
{
    public class DemoPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            if (path.EndsWith(".hgt") || path.EndsWith(".pbf") || path.EndsWith(".mapcss"))
                return path.Replace(@"\", @"/");
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace(@"\",@"/");
            // return path;
            //return String.Format("/sdcard/Mercraft/{0}", path.Replace(@"\", "/"));
        }
    }
}