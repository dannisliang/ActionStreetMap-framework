using Mercraft.Core.Tiles;

namespace Mercraft.Core.Zones
{
    public class ZoneLoadStartMessage
    {
        public Tile Tile { get; private set; }

        public ZoneLoadStartMessage(Tile tile)
        {
            Tile = tile;
        }
    }

    public class ZoneLoadFinishMessage
    {
        public Zone Zone { get; private set; }

        public ZoneLoadFinishMessage(Zone zone)
        {
            Zone = zone;
        }
    }
}
