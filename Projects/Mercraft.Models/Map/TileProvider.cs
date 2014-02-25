using System.Collections.Generic;
using Mercraft.Models.Algorithms;
using Mercraft.Models.Scene;
using UnityEngine;

namespace Mercraft.Models.Map
{
    public class TileProvider
    {
        private readonly ISceneBuilder _sceneBuilder;

        /// <summary>
        /// Stores geo coordinate for (0,0)
        /// NOTE: possible collisions due to projection precision error
        /// </summary>
        private GeoCoordinate _relativeNullPoint;

        /// <summary>
        /// Stores size of tile (Unity metric)
        /// </summary>
        private float _tileSize;

        private Dictionary<Vector2, Tile> _tiles;


        public TileProvider(ISceneBuilder sceneBuilder, 
            GeoCoordinate relativeNullPoint, float tileSize)
        {
            _sceneBuilder = sceneBuilder;
            _relativeNullPoint = relativeNullPoint;
            _tileSize = tileSize;

            _tiles = new Dictionary<Vector2, Tile>();
        }

        public Tile GetTile(Vector2 mapPoint)
        {
            // TODO mapPoint should be the center of tile!
            if (_tiles.ContainsKey(mapPoint))
                return _tiles[mapPoint];

            var coordinate = GeoProjection.ToGeoCoordinate(_relativeNullPoint, mapPoint);
            
            var bbox = BoundingBox.CreateBoundingBox(coordinate, 1000);

            var scene = _sceneBuilder.Build(coordinate, bbox);

            var tile =  new Tile(scene, coordinate, mapPoint, _tileSize);
            _tiles.Add(mapPoint, tile);

            return tile;
        }
    }
}
