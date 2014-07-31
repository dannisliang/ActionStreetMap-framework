using System.Collections.Generic;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Themes
{
    public class BuildingStyleProvider : IBuildingStyleProvider
    {
        private readonly Dictionary<string, List<BuildingStyle>> _buildingTypeStyleMapping;

        public BuildingStyleProvider(Dictionary<string, List<BuildingStyle>> buildingTypeStyleMapping)
        {
            _buildingTypeStyleMapping = buildingTypeStyleMapping;
        }

        public BuildingStyle Get(Building building)
        {
            // TODO use smart logic to choose building style
            return _buildingTypeStyleMapping[building.Type][0];
        }
    }
}
