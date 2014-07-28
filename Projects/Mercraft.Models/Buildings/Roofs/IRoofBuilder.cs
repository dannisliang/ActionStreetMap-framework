using Mercraft.Core.World.Buildings;

namespace Mercraft.Models.Buildings.Roofs
{
    public interface IRoofBuilder
    {
        MeshData Build(Building building, BuildingStyle style);
    }
}
