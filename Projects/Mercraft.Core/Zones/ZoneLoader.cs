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
        protected readonly IZoneListener ZoneListener;
        protected readonly IStylesheetProvider StylesheetProvider;
        protected readonly IGameObjectFactory GameObjectFactory;
        private readonly ISceneVisitor _sceneVisitor;

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
            IZoneListener zoneListener,
            IStylesheetProvider stylesheetProvider,
            IGameObjectFactory goFactory,
            ISceneVisitor sceneVisitor)
        {
            TileProvider = tileProvider;
            ZoneListener = zoneListener;
            StylesheetProvider = stylesheetProvider;
            GameObjectFactory = goFactory;
            _sceneVisitor = sceneVisitor;

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
            ZoneListener.OnZoneLoadStarted(tile);
            var zone = new Zone(tile, StylesheetProvider.Get(), GameObjectFactory,
                _sceneVisitor, Trace);
            zone.Build(LoadedModelIds);
            Zones.Add(tile, zone);
            CurrentZone = zone;
            ZoneListener.OnZoneLoadFinished(zone);
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