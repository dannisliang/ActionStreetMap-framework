using Mercraft.Core.Scene;

namespace Mercraft.Core.Tiles
{
    /// <summary>
    ///     Represents map tile (zone)
    /// </summary>
    public class Tile
    {
        /// <summary>
        ///     Stores map center coordinate in lat/lon
        /// </summary>
        public GeoCoordinate RelativeNullPoint { get; private set; }

        /// <summary>
        ///     Stores tile center coordinate in Unity metrics
        /// </summary>
        public MapPoint TileMapCenter { get; private set; }

        /// <summary>
        ///     Square side size in Unity metrics
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        ///     Stores scene
        /// </summary>
        public IScene Scene { get; private set; }

        public MapPoint TopLeft { get; set; }
        public MapPoint TopRight { get; set; }
        public MapPoint BottomLeft { get; set; }
        public MapPoint BottomRight { get; set; }

        public Tile(IScene scene, GeoCoordinate relativeNullPoint, MapPoint tileMapCenter, float size)
        {
            Scene = scene;
            RelativeNullPoint = relativeNullPoint;
            TileMapCenter = tileMapCenter;
            Size = size;

            TopLeft = new MapPoint(TileMapCenter.X - Size/2, TileMapCenter.Y + Size/2);
            BottomRight = new MapPoint(TileMapCenter.X + Size / 2, TileMapCenter.Y - Size / 2);

            TopRight = new MapPoint(TileMapCenter.X + Size / 2, TileMapCenter.Y + Size / 2);
            BottomLeft = new MapPoint(TileMapCenter.X - Size / 2, TileMapCenter.Y - Size / 2);
        }

        /// <summary>
        ///     Checks whether absolute position locates in tile with bound offset
        /// </summary>
        /// <param name="position">Absolute position in game</param>
        /// <param name="offset">offset from bounds</param>
        public bool Contains(MapPoint position, float offset)
        {
            var result = (position.X > TopLeft.X + offset) && (position.Y < TopLeft.Y - offset) &&
                         (position.X < BottomRight.X - offset) && (position.Y > BottomRight.Y + offset);

            return result;
        }
    }
}