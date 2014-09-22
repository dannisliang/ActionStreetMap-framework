using Mercraft.Core.World.Buildings;

namespace Mercraft.Models.Buildings.Roofs
{
    public interface IRoofBuilder
    {
        string Name { get; }
        bool CanBuild(Building building);
        MeshData Build(Building building, BuildingStyle style);
    }
}
