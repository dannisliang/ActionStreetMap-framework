
using Mercraft.Core.World.Buildings;

namespace Mercraft.Models.Buildings.Facades
{
    public interface IFacadeBuilder
    {
        string Name { get; }
        MeshData Build(Building building, BuildingStyle style);
    }
}
