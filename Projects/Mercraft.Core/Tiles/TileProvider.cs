using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Utilities;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Core.Tiles
{
    /// <summary>
    ///     This class loads and holds tiles which contain scene with models for given position
    /// </summary>
    public class TileProvider : IConfigurable
    {
        private float _tileSize;
        private float _offset;

        private readonly ISceneBuilder _sceneBuilder;
        private readonly IMessageBus _messageBus;

        // TODO Use 2d index?
        private readonly List<Tile> _tiles;

        [Dependency]
        public ITrace Trace { get; set; }

        /// <summary>
        ///     Returns loaded tile count
        /// </summary>
        public int TileCount
        {
            get { return _tiles.Count; }
        }

        [Dependency]
        public TileProvider(ISceneBuilder sceneBuilder, IMessageBus messageBus)
        {
            _sceneBuilder = sceneBuilder;
            _messageBus = messageBus;

            _tiles = new List<Tile>();
        }

        /// <summary>
        ///     Gets tile for given map position and relative null point
        /// </summary>
        public Tile GetTile(MapPoint position, GeoCoordinate relativeNullPoint)
        {
            // check whether we're in tile with offset - no need to preload tile
            Tile tile = GetTile(position, _offset);
            if (tile != null)
            {
                _messageBus.Send(new TileFoundMessage(tile, position));
                return tile;
            }

            var nextTileCenter = GetNextTileCenter(position);

            // try to find existing tile
            tile = GetTile(nextTileCenter);
            if (tile != null)
            {
                _messageBus.Send(new TileFoundMessage(tile, position));
                return tile;
            }

            // calculate geo center
            var geoCoordinate = GeoProjection.ToGeoCoordinate(relativeNullPoint, nextTileCenter);

            _messageBus.Send(new TileBuildStartMessage(nextTileCenter));

            var bbox = BoundingBox.CreateBoundingBox(geoCoordinate, _tileSize/2);
            var scene = _sceneBuilder.Build(bbox);

            tile = new Tile(scene, relativeNullPoint, nextTileCenter, _tileSize);
            scene.Canvas.Tile = tile;
            _tiles.Add(tile);

            _messageBus.Send(new TileBuildFinishMessage(tile));
            return tile;
        }

        private Tile GetTile(MapPoint position, float offset)
        {
            return _tiles.FirstOrDefault(t => t.Contains(position, offset));
        }

        private Tile GetTile(MapPoint tileCenter)
        {
            return _tiles.SingleOrDefault(t => tileCenter.AreSame(t.MapCenter));
        }

        private MapPoint GetNextTileCenter(MapPoint position)
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
            if (IsPointInTreangle(position, tile.MapCenter, tile.TopLeft, tile.TopRight))
                return new MapPoint(tile.MapCenter.X, tile.MapCenter.Y + _tileSize);

            // left
            if (IsPointInTreangle(position, tile.MapCenter, tile.TopLeft, tile.BottomLeft))
                return new MapPoint(tile.MapCenter.X - _tileSize, tile.MapCenter.Y);

            // right
            if (IsPointInTreangle(position, tile.MapCenter, tile.TopRight, tile.BottomRight))
                return new MapPoint(tile.MapCenter.X + _tileSize, tile.MapCenter.Y);

            // bottom
            return new MapPoint(tile.MapCenter.X, tile.MapCenter.Y - _tileSize);
        }

        /// <summary>
        ///     Checks whether point is located in triangle
        ///     http://stackoverflow.com/questions/13300904/determine-whether-point-lies-inside-triangle
        /// </summary>
        private bool IsPointInTreangle(MapPoint p, MapPoint p1, MapPoint p2, MapPoint p3)
        {
            float alpha = ((p2.Y - p3.Y)*(p.X - p3.X) + (p3.X - p2.X)*(p.Y - p3.Y))/
                          ((p2.Y - p3.Y)*(p1.X - p3.X) + (p3.X - p2.X)*(p1.Y - p3.Y));
            float beta = ((p3.Y - p1.Y)*(p.X - p3.X) + (p1.X - p3.X)*(p.Y - p3.Y))/
                         ((p2.Y - p3.Y)*(p1.X - p3.X) + (p3.X - p2.X)*(p1.Y - p3.Y));
            float gamma = 1.0f - alpha - beta;

            return alpha > 0 && beta > 0 && gamma > 0;
        }

        /// <summary>
        ///     Configures class
        /// </summary>
        public void Configure(IConfigSection configSection)
        {
            _tileSize = configSection.GetFloat("@size");
            _offset = configSection.GetFloat("@offset");
        }
    }
}