
using UnityEngine;

namespace Mercraft.Core.Tiles
{
    public interface ITileListener
    {
        void OnTileLoadStarted(Vector2 center, GeoCoordinate relativeNullPoint);
        void OnTileLoadFinished(Tile tile);
    }
}
