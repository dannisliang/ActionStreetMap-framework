using Mercraft.Core;

namespace Mercraft.Models.Terrain
{
    public class AreaSettings
    {
        public const int DefaultIndex = -1;

        public float ZIndex;
        public int SplatIndex;
        public int DetailIndex = DefaultIndex;
        public MapPoint[] Points;
    }
}