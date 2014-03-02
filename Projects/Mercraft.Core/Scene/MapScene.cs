using System.Collections.Generic;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public class MapScene : IScene
    {
        private List<Building> _buildings;
        public IEnumerable<Building> Buildings
        {
            get
            {
                return _buildings;
            }
        }

        public MapScene()
        {
            _buildings = new List<Building>();
        }

        public void AddBuilding(Building building)
        {
            _buildings.Add(building);
        }
    }
}
