using System.Collections.Generic;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class ZoneLoader
    {
        private readonly TileProvider _tileProvider;
        private readonly IEnumerable<IGameObjectBuilder> _gameOnObjectBuilders;

        [Dependency]
        public ZoneLoader(TileProvider tileProvider, IEnumerable<IGameObjectBuilder> gameOnObjectBuilders)
        {
            _tileProvider = tileProvider;
            _gameOnObjectBuilders = gameOnObjectBuilders;
        }

        public void Load(Vector2 position)
        {
            var tile = _tileProvider.GetTile(position);

           /* foreach (var gameOnObjectBuilder in _gameOnObjectBuilders)
            {
                gameOnObjectBuilder.Build()
            }*/

        }
    }
}
