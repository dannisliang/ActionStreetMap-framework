using Mercraft.Core.World.Roads;

namespace Mercraft.Models.Roads
{
    public interface IRoadStyleProvider
    {
        RoadStyle Get(Road road);
    }
}