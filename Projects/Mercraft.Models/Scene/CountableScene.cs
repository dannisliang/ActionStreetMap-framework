using System.Collections.Generic;

namespace Mercraft.Models.Scene
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
