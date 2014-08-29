using Mercraft.Core;

namespace Mercraft.Models.Terrain
{
    public class HeightMap
    {
        public GeoCoordinate Center { get; set; }
        public int Resolution { get; set; }
        public float Size { get; set; }
        public float MaxElevation { get; set; }

        public bool IsFlat { get; set; }

        public float[,] Map { get; set; }
    }
}
