using System;
using System.Collections.Generic;

namespace Mercraft.Core.World.Buildings
{
    public class BuildingCollection
    {
        private Dictionary<long, Building> _buildings = new Dictionary<long, Building>();

        public Building Find(long id)
        {
            return _buildings[id];
        }

        public void Insert(Building building)
        {
            _buildings.Add(building.Id, building);
        }
    }
}
