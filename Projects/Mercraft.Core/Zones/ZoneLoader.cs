using System.Collections.Generic;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class ZoneLoader: IPositionListener
    {
        private readonly TileProvider _tileProvider;
        private readonly IEnumerable<IGameObjectBuilder> _gameOnObjectBuilders;

        [Dependency]
        public ZoneLoader(TileProvider tileProvider/*, IEnumerable<IGameObjectBuilder> gameOnObjectBuilders*/)
        {
            _tileProvider = tileProvider;
            //_gameOnObjectBuilders = gameOnObjectBuilders;
        }

        public void OnPositionChanged(Vector2 position)
        {
            // Load zone if needed
            //var tile = _tileProvider.GetTile(position);
            // TODO
        }
    }
}
