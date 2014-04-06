using UnityEngine;

namespace Mercraft.Core
{
    public interface IPositionListener
    {
        GeoCoordinate RelativeNullPoint { get; }
        void OnMapPositionChanged(Vector2 position);
        void OnGeoPositionChanged(GeoCoordinate position);
    }
}
