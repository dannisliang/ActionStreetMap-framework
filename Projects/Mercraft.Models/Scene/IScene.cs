
using System.Collections.Generic;

namespace Mercraft.Models.Scene
{
    public interface IScene
    {
        void AddBuilding(Building building);

        IEnumerable<Building> Buildings { get; }
    }
}
