using System.Collections.Generic;
using Mercraft.Models;

namespace Mercraft.Maps.UnitTests.Stubs
{
    public class CountableScene: IScene
    {
        public List<Building> Buildings { get; set; }

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
