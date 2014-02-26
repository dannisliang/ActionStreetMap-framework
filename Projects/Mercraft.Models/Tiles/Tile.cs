using Mercraft.Models.Scene;
using UnityEngine;

namespace Mercraft.Models.Tiles
{
    /// <summary>
    /// Represents map tile (zone)
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Stores tile center coordinate in lat/lon
        /// </summary>
        public GeoCoordinate TileGeoCenter { get; private set; }

        /// <summary>
        /// Stores tile center coordinate in Unity metrics
        /// </summary>
        public Vector3 TileMapCenter { get; private set; }

        /// <summary>
        /// Square side size in Unity metrics
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        /// Stores scene
        /// </summary>
        public IScene Scene { get; private set; }

        public Tile(IScene scene, GeoCoordinate tileGeoCenter, Vector2 tileMapCenter, float size)
        {
            Scene = scene;
            TileGeoCenter = tileGeoCenter;
            TileMapCenter = tileMapCenter;
            Size = size;
        }
    }
}
