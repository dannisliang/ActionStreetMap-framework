using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Utilities;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Core.Tiles
{
    /// <summary>
    ///     This class loads and holds tiles which contain scene with models for given position
    /// </summary>
    public class TileProvider : IConfigurable
    {
        private readonly string LogTag = typeof (TileProvider).Name;
        private float _tileSize;
        private float _offset;

        private readonly ISceneBuilder _sceneBuilder;
        private readonly ITileListener _tileListener;

        private readonly List<Tile> _tiles;

        [Dependency]
        public ITrace Trace { get; set; }

        [Dependency]
        public TileProvider(ISceneBuilder sceneBuilder, ITileListener tileListener)
        {
            _sceneBuilder = sceneBuilder;
            _tileListener = tileListener;

            _tiles = new List<Tile>();
        }

        /// <summary>
        ///     Gets tile for given map position and relative null point
        /// </summary>
        public Tile GetTile(Vector2 position, GeoCoordinate relativeNullPoint)
        {
            // check whether we're in tile with offset - no need to preload tile
            Tile tile = GetTile(position, _offset);
            if (tile != null)
            {
                LogTileFound(tile, position);
                return tile;
            }

            var nextTileCenter = GetNextTileCenter(position);

            // try to find existing tile
            tile = GetTile(nextTileCenter);
            if (tile != null)
            {
                LogTileFound(tile, position);
                return tile;
            }

            // calculate geo center
            var geoCoordinate = GeoProjection.ToGeoCoordinate(relativeNullPoint, nextTileCenter);

            _tileListener.OnTileLoadStarted(nextTileCenter, relativeNullPoint);

            var bbox = BoundingBox.CreateBoundingBox(geoCoordinate, _tileSize/2);
            var scene = _sceneBuilder.Build(bbox);
           
            tile = new Tile(scene, relativeNullPoint, nextTileCenter, _tileSize);
            scene.Canvas.Tile = tile;
            _tiles.Add(tile);
            _tileListener.OnTileLoadFinished(tile);
            return tile;
        }

        private Tile GetTile(Vector2 position, float offset)
        {
            // TODO change to FirstOrDefault after ensure that only one tile is found
            return _tiles.SingleOrDefault(t => t.Contains(position, offset));
        }

        private Tile GetTile(Vector2 tileCenter)
        {
            return _tiles.SingleOrDefault(t => tileCenter.AreSame(t.TileMapCenter));
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

            // top
            if (IsPointInTreangle(position, tile.TileMapCenter, tile.TopLeft, tile.TopRight))
                return new Vector2(tile.TileMapCenter.x, tile.TileMapCenter.y + _tileSize);

            // left
            if (IsPointInTreangle(position, tile.TileMapCenter, tile.TopLeft, tile.BottomLeft))
                return new Vector2(tile.TileMapCenter.x - _tileSize, tile.TileMapCenter.y);

            // right
            if (IsPointInTreangle(position, tile.TileMapCenter, tile.TopRight, tile.BottomRight))
                return new Vector2(tile.TileMapCenter.x + _tileSize, tile.TileMapCenter.y);

            // bottom
            return new Vector2(tile.TileMapCenter.x, tile.TileMapCenter.y - _tileSize);
        }

        /// <summary>
        ///     Checks whether point is located in triangle
        ///     http://stackoverflow.com/questions/13300904/determine-whether-point-lies-inside-triangle
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

        private void LogTileFound(Tile tile, Vector2 position)
        {
            Trace.Normal(LogTag,
                String.Format("Position {0} is found in tile with center {1}", position, tile.TileMapCenter));
        }

        /// <summary>
        ///     Configures class
        /// </summary>
        public void Configure(IConfigSection configSection)
        {
            _tileSize = configSection.GetFloat("tile/@size");
            _offset = configSection.GetFloat("tile/@offset");
        }
    }
}