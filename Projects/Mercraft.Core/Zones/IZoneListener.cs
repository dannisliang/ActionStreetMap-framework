using Mercraft.Core.Tiles;

namespace Mercraft.Core.Zones
{
    /// <summary>
    ///     Provides the way to listen events of zone processing
    /// </summary>
    public interface IZoneListener
    {
        /// <summary>
        ///     Called when Zone loading begins
        /// </summary>
        /// <param name="tile">Tile which should be loaded</param>
        void OnZoneLoadStarted(Tile tile);

        /// <summary>
        ///     Called when zone is loaded
        /// </summary>
        /// <param name="zone">Loaded zone</param>
        void OnZoneLoadFinished(Zone zone);
    }
}