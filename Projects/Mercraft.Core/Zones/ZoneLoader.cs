using System.Collections.Generic;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class ZoneLoader: IPositionListener, IConfigurable
    {
        private const string OffsetKey = "offset";

        protected readonly TileProvider TileProvider;
        protected readonly IStylesheetProvider StylesheetProvider;
        protected readonly IGameObjectBuilder SceneModelVisitor;
       
        protected float Offset { get; set; }

        protected readonly HashSet<long> LoadedModelIds;

        public GeoCoordinate RelativeNullPoint { get; private set; }
        public Vector2 CurrentPosition { get; private set; }
        public Zone CurrentZone { get; private set; }

        protected Dictionary<Tile, Zone> Zones { get; set; }

        [Dependency]
        protected ITrace Trace { get; set; }

        [Dependency]
        public ZoneLoader(TileProvider tileProvider, 
            IStylesheetProvider stylesheetProvider,
            IGameObjectBuilder sceneModelVisitor)
        {
            TileProvider = tileProvider;
            StylesheetProvider = stylesheetProvider;
            SceneModelVisitor = sceneModelVisitor;

            LoadedModelIds = new HashSet<long>();
            Zones = new Dictionary<Tile, Zone>();
            CurrentPosition = new Vector2();
        }

        public virtual void OnMapPositionChanged(Vector2 position)
        {
            CurrentPosition = position;
            var tile = TileProvider.GetTile(position, RelativeNullPoint);

            if (Zones.ContainsKey(tile))
            {
                CurrentZone = Zones[tile];
                return;
            }

            // Build zone
            var zone = new Zone(tile, StylesheetProvider.Get(), SceneModelVisitor, Trace);
            zone.Build(LoadedModelIds);
            Zones.Add(tile, zone);
            CurrentZone = zone;
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
            Offset = configSection.GetFloat(OffsetKey);
        }
    }
}
