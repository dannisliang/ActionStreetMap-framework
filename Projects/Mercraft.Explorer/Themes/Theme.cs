using System.Collections.Generic;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Themes
{
    public class Theme
    {
        public string Name { get; set; }

        public Dictionary<string, List<BuildingStyle>> BuildingTypeStyleMapping { get; set; }
    }
}
