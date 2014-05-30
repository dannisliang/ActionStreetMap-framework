
namespace Mercraft.Core.Tiles
{
    /// <summary>
    ///     Provides the way to listen events of tile processing
    /// </summary>
    public interface ITileListener
    {
        /// <summary>
        ///     Called when tile loading begins
        /// </summary>
        /// <param name="center">Tile map center</param>
        /// <param name="relativeNullPoint">Relative null point</param>
        void OnTileLoadStarted(MapPoint center, GeoCoordinate relativeNullPoint);

        /// <summary>
        ///     Called when tile is loaded
        /// </summary>
        /// <param name="tile">Loaded tile</param>
        void OnTileLoadFinished(Tile tile);

        /// <summary>
        ///     Called when tile is found in previously loaded collection
        /// </summary>
        /// <param name="tile">Actual tile</param>
        /// <param name="position">Actual position</param>
        void OnTileFound(Tile tile, MapPoint position);
    }
}