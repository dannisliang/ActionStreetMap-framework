using System.Collections.Generic;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class ZoneLoader: IPositionListener, IConfigurable
    {
        protected readonly TileProvider TileProvider;
        protected readonly IFloorBuilder FloorBuilder;
        protected readonly IEnumerable<ISceneModelVisitor> SceneModelVisitors;

        protected Vector2 CurrentPosition { get; set; }
        protected float Offset { get; set; }
        protected GeoCoordinate RelativeNullPoint { get; set; }

        protected Dictionary<Tile, Zone> Zones { get; set; }

        [Dependency]
        public ZoneLoader(TileProvider tileProvider, 
            IFloorBuilder floorBuilder,
            IEnumerable<ISceneModelVisitor> sceneModelVisitors)
        {
            TileProvider = tileProvider;
            FloorBuilder = floorBuilder;
            SceneModelVisitors = sceneModelVisitors;

            Zones = new Dictionary<Tile, Zone>();
            CurrentPosition = new Vector2();
        }

        public virtual void OnMapPositionChanged(Vector2 position)
        {
            var tile = TileProvider.GetTile(position, RelativeNullPoint);

            if(Zones.ContainsKey(tile))
                return;

            // Build zone
            var zone = new Zone(tile, FloorBuilder, SceneModelVisitors);
            zone.Build();
            Zones.Add(tile, zone);           
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
            Offset = configSection.GetFloat("offset");
        }
    }
}
