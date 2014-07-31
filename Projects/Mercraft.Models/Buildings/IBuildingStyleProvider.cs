using Mercraft.Core.World.Buildings;

namespace Mercraft.Models.Buildings
{
    public interface IBuildingStyleProvider
    {
        BuildingStyle Get(Building building);
    }
}