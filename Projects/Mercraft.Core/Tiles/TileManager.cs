using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Utilities;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Core.Tiles
{
    /// <summary>
    ///     This class loads and holds tiles which contain scene with models for given position
    /// </summary>
    public class TileManager : IPositionListener, IConfigurable
    {
        private float _tileSize;
        private float _offset;
        private int _heightmapsize;

        private readonly ISceneBuilder _sceneBuilder;
        private readonly IModelVisitor _modelVisitor;
        private readonly IMessageBus _messageBus;
        private readonly IHeightMapProvider _heightMapProvider;

        public GeoCoordinate RelativeNullPoint { get; private set; }
        public MapPoint CurrentPosition { get; private set; }
        public Tile CurrentTile { get; private set; }

        // NOTE use 2d index?
        protected HashSet<Tile> Tiles { get; set; }

        [Dependency]
        private ITrace Trace { get; set; }

        /// <summary>
        ///     Returns loaded tile count
        /// </summary>
        public int TileCount
        {
            get { return Tiles.Count; }
        }

        [Dependency]
        public TileManager(ISceneBuilder sceneBuilder, IModelVisitor modelVisitor, 
            IHeightMapProvider heightMapProvider, IMessageBus messageBus)
        {
            _sceneBuilder = sceneBuilder;
            _modelVisitor = modelVisitor;
            _messageBus = messageBus;
            _heightMapProvider = heightMapProvider;

            Tiles = new HashSet<Tile>();
        }

        #region IPositionListener

        public virtual void OnMapPositionChanged(MapPoint position)
        {
            CurrentPosition = position;
            var tile = GetTile(position, RelativeNullPoint);
            CurrentTile = tile;

            if (Tiles.Contains(tile))
                return;

            Tiles.Add(tile);
        }

        public virtual void OnGeoPositionChanged(GeoCoordinate position)
        {
            RelativeNullPoint = position;

            // TODO need think about this
            // TODO Destroy existing!
            Tiles.Clear();
        }

        #endregion

        #region Tile provider logic

        /// <summary>
        ///     Gets tile for given map position and relative null point
        /// </summary>
        private Tile GetTile(MapPoint position, GeoCoordinate relativeNullPoint)
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

            tile = new Tile(relativeNullPoint, nextTileCenter, _tileSize);
            tile.HeightMap = _heightMapProvider.Get(tile, _heightmapsize);
            
            _sceneBuilder.Build(tile, bbox);
            
            _messageBus.Send(new TileBuildFinishMessage(tile));
            return tile;
        }

        private Tile GetTile(MapPoint position, float offset)
        {
            return Tiles.FirstOrDefault(t => t.Contains(position, offset));
        }

        private Tile GetTile(MapPoint tileCenter)
        {
            return Tiles.SingleOrDefault(t => tileCenter.AreSame(t.MapCenter));
        }

        private MapPoint GetNextTileCenter(MapPoint position)
        {
            // No tiles so far, create default using current position as center
            if (!Tiles.Any())
                return position;

            // NOTE we assume that there are no instant position changing
            // so the call with 0 offset will give us current tile
            var tile = GetTile(position, 0);
            if (tile == null)
                throw new InvalidOperationException("Instant position changing detected!");

            // top
            if (GeometryUtils.IsPointInTreangle(position, tile.MapCenter, tile.TopLeft, tile.TopRight))
                return new MapPoint(tile.MapCenter.X, tile.MapCenter.Y + _tileSize);

            // left
            if (GeometryUtils.IsPointInTreangle(position, tile.MapCenter, tile.TopLeft, tile.BottomLeft))
                return new MapPoint(tile.MapCenter.X - _tileSize, tile.MapCenter.Y);

            // right
            if (GeometryUtils.IsPointInTreangle(position, tile.MapCenter, tile.TopRight, tile.BottomRight))
                return new MapPoint(tile.MapCenter.X + _tileSize, tile.MapCenter.Y);

            // bottom
            return new MapPoint(tile.MapCenter.X, tile.MapCenter.Y - _tileSize);
        }    

        #endregion

        /// <summary>
        ///     Configures class
        /// </summary>
        public void Configure(IConfigSection configSection)
        {
            _tileSize = configSection.GetFloat("@size");
            _offset = configSection.GetFloat("@offset");
            _heightmapsize = configSection.GetInt("@heightmap");

            RelativeNullPoint = new GeoCoordinate(
              configSection.GetFloat("@latitude"),
              configSection.GetFloat("@longitude"));
        }
    }
}