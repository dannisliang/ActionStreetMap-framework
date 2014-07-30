using System.Collections.Generic;
using Mercraft.Models.Buildings;
using Mercraft.Models.Roads;

namespace Mercraft.Explorer.Themes
{
    public class Theme
    {
        public string Name { get; set; }

        public Dictionary<string, List<BuildingStyle>> BuildingTypeStyleMapping { get; set; }
        public Dictionary<string, List<RoadStyle>> RoadTypeStyleMapping { get; set; }
    }
}
