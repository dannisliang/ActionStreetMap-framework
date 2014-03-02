using Mercraft.Core.Scene;
using UnityEngine;

namespace Mercraft.Core.Tiles
{
    /// <summary>
    /// Represents map tile (zone)
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Stores map center coordinate in lat/lon
        /// </summary>
        public GeoCoordinate RelativeNullPoint { get; private set; }

        /// <summary>
        /// Stores tile center coordinate in Unity metrics
        /// </summary>
        public Vector2 TileMapCenter { get; private set; }

        /// <summary>
        /// Square side size in Unity metrics
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        /// Stores scene
        /// </summary>
        public IScene Scene { get; private set; }

        public Vector2 TopLeft { get; set; }
        public Vector2 TopRight { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 BottomRight { get; set; }

        public Tile(IScene scene, GeoCoordinate relativeNullPoint, Vector2 tileMapCenter, float size)
        {
            Scene = scene;
            RelativeNullPoint = relativeNullPoint;
            TileMapCenter = tileMapCenter;
            Size = size;

            TopLeft = new Vector2(TileMapCenter.x - Size / 2, TileMapCenter.y + Size / 2);
            BottomRight = new Vector2(TileMapCenter.x + Size / 2, TileMapCenter.y - Size / 2);

            TopRight = new Vector2(TileMapCenter.x + Size / 2, TileMapCenter.y + Size / 2);
            BottomLeft = new Vector2(TileMapCenter.x - Size / 2, TileMapCenter.y - Size / 2);
        }

        /// <summary>
        /// Checks whether absolute position locates in tile with bound offset
        /// </summary>
        /// <param name="position">Absolute position in game</param>
        /// <param name="offset">offset from bounds</param>
        public bool Contains(Vector2 position, float offset)
        {
            var result = (position.x > TopLeft.x + offset) && (position.y < TopLeft.y - offset) &&
                   (position.x < BottomRight.x - offset) && (position.y > BottomRight.y + offset);

            return result;
        }
    }
}
