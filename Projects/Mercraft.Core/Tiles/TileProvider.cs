using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Algorithms;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Core.Tiles
{
    public class TileProvider: IConfigurable
    {
        private readonly string LogTag = typeof (TileProvider).Name;
        private float _tileSize;
        private float _offset;

        private readonly ISceneBuilder _sceneBuilder;
        private readonly List<Tile> _tiles;

        [Dependency]
        private ITrace Trace { get; set; }

        [Dependency]
        public TileProvider(ISceneBuilder sceneBuilder)
        {
            _sceneBuilder = sceneBuilder;
            _tiles = new List<Tile>();
        }

        /*public bool IsBoundPosition(Vector2 position)
        {
            Tile tile = GetTile(position, _offset);
            return tile == null;
        }*/

        public Tile GetTile(Vector2 position, GeoCoordinate relativeNullPoint)
        {
            // check whether we're in tile with offset - no need to preload tile
            Tile tile = GetTile(position, _offset);
            if (tile != null)
            {
                Trace.Info(LogTag, String.Format("Position {0} is found in tile with center {1}", position, tile.TileMapCenter));
                return tile;
            }

            var nextTileCenter = GetNextTileCenter(position);

            // calculate geo center
            var geoCoordinate = GeoProjection.ToGeoCoordinate(relativeNullPoint, nextTileCenter);
            
            var bbox = BoundingBox.CreateBoundingBox(geoCoordinate, _tileSize / 2);

            var scene = _sceneBuilder.Build(geoCoordinate, bbox);

            tile = new Tile(scene, geoCoordinate, nextTileCenter, _tileSize);
            _tiles.Add(tile);
            Trace.Info(LogTag, String.Format("Created tile with center: ({0},{1}), size:{2}, geo: {3}:{4}",
                nextTileCenter.x, nextTileCenter.y, _tileSize, geoCoordinate.Latitude, geoCoordinate.Longitude));
            return tile;
        }

        private Tile GetTile(Vector2 position, float offset)
        {
            // TODO change to FirstOrDefault after ensure that only one tile is found
            return _tiles.SingleOrDefault(t => t.Contains(position, offset));
        }

        private Vector2 GetNextTileCenter(Vector2 position)
        {
            // No tiles so far, create default using current position as center
            if (!_tiles.Any())
                return position;

            // NOTE we assume that there are no instant position changing
            // so the call with 0 offset will give us current tile
            var tile = GetTile(position, 0);
            if (tile == null)
                throw new InvalidOperationException("Instant position changing detected!");

            var direction = GetDirection(tile, position);
            Trace.Info("", String.Format("Next tile position: {0}", position));
            switch (direction)
            {
                case Direction.Left:
                    return new Vector2(tile.TileMapCenter.x - _tileSize, tile.TileMapCenter.y);
                case Direction.Right:
                    return new Vector2(tile.TileMapCenter.x + _tileSize, tile.TileMapCenter.y);
                case Direction.Top:
                    return new Vector2(tile.TileMapCenter.x, tile.TileMapCenter.y + _tileSize);
                case Direction.Bottom:
                    return new Vector2(tile.TileMapCenter.x, tile.TileMapCenter.y - _tileSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Determine tile direction using current position
        /// </summary>
        private Direction GetDirection(Tile tile, Vector2 position)
        {
            if (IsPointInTreangle(position, tile.TileMapCenter, tile.TopLeft, tile.TopRight))
                return Direction.Top;

            if (IsPointInTreangle(position, tile.TileMapCenter, tile.TopLeft, tile.BottomRight))
                return Direction.Left;

            if (IsPointInTreangle(position, tile.TileMapCenter, tile.TopRight, tile.BottomRight))
                return Direction.Right;

            return Direction.Bottom;
        }

        /// <summary>
        /// Checks whether point is located in triangle
        /// http://stackoverflow.com/questions/13300904/determine-whether-point-lies-inside-triangle
        /// </summary>
        private bool IsPointInTreangle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float alpha = ((p2.y - p3.y)*(p.x - p3.x) + (p3.x - p2.x)*(p.y - p3.y))/
                          ((p2.y - p3.y)*(p1.x - p3.x) + (p3.x - p2.x)*(p1.y - p3.y));
            float beta = ((p3.y - p1.y)*(p.x - p3.x) + (p1.x - p3.x)*(p.y - p3.y))/
                         ((p2.y - p3.y)*(p1.x - p3.x) + (p3.x - p2.x)*(p1.y - p3.y));
            float gamma = 1.0f - alpha - beta;

            return alpha > 0 && beta > 0 && gamma > 0;
        }

        public void Configure(IConfigSection configSection)
        {
            _tileSize = configSection.GetFloat("tile/@size");
            _offset = configSection.GetFloat("tile/@offset");
        }

        private enum Direction
        {
            Left,
            Right,
            Top,
            Bottom
        }
    }
}
