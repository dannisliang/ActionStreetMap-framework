
namespace Mercraft.Core
{
    /// <summary>
    /// Represents listener for map and geo coordinate changes
    /// </summary>
    public interface IPositionListener
    {
        /// <summary>
        /// This geo coordinate is mapped to (0, 0, 0) map coordinate
        /// </summary>
        GeoCoordinate RelativeNullPoint { get; }

        /// <summary>
        /// Called when map position is changed. It should occur when character moves
        /// </summary>
        void OnMapPositionChanged(MapPoint position);

        /// <summary>
        /// Called when new relative null point is set. It should trigger logic which destroys
        /// built world
        /// </summary>
        void OnGeoPositionChanged(GeoCoordinate position);
    }
}
