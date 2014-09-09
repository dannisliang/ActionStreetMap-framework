using System.Collections.Generic;
using Mercraft.Core.World.Buildings;
using Mercraft.Core.World.Roads;

namespace Mercraft.Core.World
{
    /// <summary>
    ///     Holds all game models to allow special processing on them depending on game needs
    /// </summary>
    public class WorldManager
    {
        private readonly HashSet<long>  _globalIds = new HashSet<long>();

        public void AddBuilding(Building building)
        {
            _globalIds.Add(building.Id);
        }

        public void AddRoad(Road road)
        {
            road.Elements.ForEach(e => _globalIds.Add(e.Id));
        }

        public void AddModel(long id)
        {
            _globalIds.Add(id);
        }

        public bool Contains(long id)
        {
            return _globalIds.Contains(id);
        }
    }
}