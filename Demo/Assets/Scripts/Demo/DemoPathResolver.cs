using System;
using Mercraft.Infrastructure.Config;

namespace Assets.Scripts.Demo
{
    public class DemoPathResolver : IPathResolver
    {
        public string Resolve(string path)
        {
            return String.Format("/sdcard/Mercraft/{0}", path.Replace(@"\", "/"));
            //return path;
        }
    }
}