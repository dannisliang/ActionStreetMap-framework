using Mercraft.Core.World.Roads;
using Mercraft.Models.Roads;

namespace Mercraft.Explorer.Themes
{
    public interface IRoadStyleProvider
    {
        RoadStyle Get(Theme theme, Road road);
    }

    public class RoadStyleProvider : IRoadStyleProvider
    {
        public RoadStyle Get(Theme theme, Road road)
        {
            // NOTE use first element's type
            var type = road.Elements[0].Type;

            // TODO use smart logic to choose road style
            return theme.RoadTypeStyleMapping[type][0];
        }
    }
}
