using UnityEngine;

namespace Mercraft.Core
{
    public interface IPositionListener
    {
        void OnMapPositionChanged(Vector2 position);
        void OnGeoPositionChanged(GeoCoordinate position);
    }
}
