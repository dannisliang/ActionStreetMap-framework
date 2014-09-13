using Mercraft.Core;

namespace Mercraft.Models.Terrain
{
    public class AreaSettings
    {
        public float ZIndex;
        public int SplatIndex;
        public int DetailIndex = -1;
        public MapPoint[] Points;
    }
}