using System.Collections.Generic;

namespace Mercraft.Models.Scene
{
    public class CountableScene : IScene
    {
        public List<Building> Buildings { get; private set; }

        public CountableScene()
        {
            Buildings = new List<Building>();
        }

        public void AddBuilding(Building building)
        {
            Buildings.Add(building);
        }
    }
}
