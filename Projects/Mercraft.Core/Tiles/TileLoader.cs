using System.Collections.Generic;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Core.Tiles
{
    public class TileLoader : IPositionListener, IConfigurable
    {
        private readonly TileProvider _tileProvider;
        private readonly ITileVisitor _tileVisitor;
        private readonly IMessageBus _messageBus;

        protected float Offset { get; set; }

        public GeoCoordinate RelativeNullPoint { get; private set; }
        public MapPoint CurrentPosition { get; private set; }
        public Tile CurrentTile { get; private set; }

        protected HashSet<Tile> Tiles { get; set; }

        [Dependency]
        protected ITrace Trace { get; set; }

        [Dependency]
        public TileLoader(TileProvider tileProvider, ITileVisitor tileVisitor, IMessageBus messageBus)
        {
            _tileProvider = tileProvider;
            _tileVisitor = tileVisitor;
            _messageBus = messageBus;

            Tiles = new HashSet<Tile>();
            CurrentPosition = new MapPoint();
        }

        #region IPositionListener implementation

        public virtual void OnMapPositionChanged(MapPoint position)
        {
            CurrentPosition = position;
            var tile = _tileProvider.GetTile(position, RelativeNullPoint);
            CurrentTile = tile;
            
            if (Tiles.Contains(tile))
                return;

            _messageBus.Send(new TileLoadStartMessage(tile));
            _tileVisitor.Visit(tile);
            Tiles.Add(tile);
            _messageBus.Send(new TileLoadFinishMessage(tile));
        }

        public virtual void OnGeoPositionChanged(GeoCoordinate position)
        {
            RelativeNullPoint = position;

            // TODO need think about this
            // TODO Destroy existing!
            Tiles = new HashSet<Tile>();
        }

        #endregion

        #region IConfigurable implementation

        public void Configure(IConfigSection configSection)
        {
            RelativeNullPoint = new GeoCoordinate(
                configSection.GetFloat("@latitude"),
                configSection.GetFloat("@longitude"));
        }
        #endregion

    }
}
