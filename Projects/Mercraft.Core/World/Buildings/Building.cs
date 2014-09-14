using Mercraft.Core.Unity;

namespace Mercraft.Core.World.Buildings
{
    public class Building
    {
        public long Id { get; set; }

        public bool IsDestroyed { get; set; }

        public Address Address { get; set; }

        public IGameObject GameObject { get; set; }

        public float Height { get; set; }

        public float MinHeight { get; set; }

        public int Levels { get; set; }

        public float Elevation { get; set; }

        public MapPoint[] Footprint { get; set; }

        public string Type { get; set; }
    }
}
