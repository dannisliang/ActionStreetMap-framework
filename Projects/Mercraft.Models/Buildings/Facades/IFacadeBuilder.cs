
using Mercraft.Core.World.Buildings;

namespace Mercraft.Models.Buildings.Facades
{
    public interface IFacadeBuilder
    {
        MeshData Build(Building building, BuildingStyle style);
    }
}
