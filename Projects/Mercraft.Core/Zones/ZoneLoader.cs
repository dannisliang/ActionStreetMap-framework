using System.Collections.Generic;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Core.Zones
{
    /// <summary>
    ///     This class is responsible for loading different zones as response to position changes
    /// </summary>
    public class ZoneLoader : IPositionListener, IConfigurable
    {
        protected readonly TileProvider TileProvider;
        protected readonly IStylesheetProvider StylesheetProvider;
        protected readonly IGameObjectFactory GameObjectFactory;
        private readonly ISceneVisitor _sceneVisitor;
        private readonly IMessageBus _messageBus;

        protected float Offset { get; set; }

        protected readonly HashSet<long> LoadedModelIds;

        public GeoCoordinate RelativeNullPoint { get; private set; }
        public MapPoint CurrentPosition { get; private set; }
        public Zone CurrentZone { get; private set; }

        protected Dictionary<Tile, Zone> Zones { get; set; }

        [Dependency]
        protected ITrace Trace { get; set; }

        [Dependency]
        public ZoneLoader(TileProvider tileProvider,
            IStylesheetProvider stylesheetProvider,
            IGameObjectFactory goFactory,
            ISceneVisitor sceneVisitor,
            IMessageBus messageBus)
        {
            TileProvider = tileProvider;
            StylesheetProvider = stylesheetProvider;
            GameObjectFactory = goFactory;
            _sceneVisitor = sceneVisitor;
            _messageBus = messageBus;

            LoadedModelIds = new HashSet<long>();
            Zones = new Dictionary<Tile, Zone>();
            CurrentPosition = new MapPoint();
        }

        public virtual void OnMapPositionChanged(MapPoint position)
        {
            CurrentPosition = position;
            var tile = TileProvider.GetTile(position, RelativeNullPoint);

            if (Zones.ContainsKey(tile))
            {
                CurrentZone = Zones[tile];
                return;
            }

            // Build zone
            _messageBus.Send(new ZoneLoadStartMessage(tile));
            var zone = new Zone(tile, StylesheetProvider.Get(), GameObjectFactory,
                _sceneVisitor, Trace);
            zone.Build(LoadedModelIds);
            Zones.Add(tile, zone);
            CurrentZone = zone;
            _messageBus.Send(new ZoneLoadFinishMessage(zone));
        }

        public virtual void OnGeoPositionChanged(GeoCoordinate position)
        {
            RelativeNullPoint = position;

            // TODO need think about this
            // TODO Destroy existing!
            Zones = new Dictionary<Tile, Zone>();
        }

        public void Configure(IConfigSection configSection)
        {
            RelativeNullPoint = new GeoCoordinate(
                configSection.GetFloat("@latitude"),
                configSection.GetFloat("@longitude"));
        }
    }
}