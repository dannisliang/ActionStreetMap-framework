using System.Collections.Generic;
using Mercraft.Core.World.Infos;
using Mercraft.Models.Infos;

namespace Mercraft.Explorer.Themes
{
    public class InfoStyleProvider: IInfoStyleProvider
    {
        private readonly Dictionary<string, InfoStyle> _infoStyleMap;

        public InfoStyleProvider(Dictionary<string, InfoStyle> infoStyleMap)
        {
            _infoStyleMap = infoStyleMap;
        }

        public InfoStyle Get(Info info)
        {
            return _infoStyleMap[info.Key];
        }
    }
}
