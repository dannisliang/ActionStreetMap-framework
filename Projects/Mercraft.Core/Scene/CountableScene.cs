using System.Collections.Generic;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public class CountableScene : IScene
    {
        private List<Building> _buildings;
        public IEnumerable<Building> Buildings
        {
            get
            {
                return _buildings;
            }
        }

        public CountableScene()
        {
            _buildings = new List<Building>();
        }

        public void AddBuilding(Building building)
        {
            _buildings.Add(building);
        }
    }
}
