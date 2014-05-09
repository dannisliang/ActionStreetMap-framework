using Mercraft.Core.Tiles;

namespace Mercraft.Core.Zones
{
    public interface IZoneListener
    {
        void OnZoneLoadStarted(Tile tile);
        void OnZoneLoadFinished(Zone zone);
    }
}
