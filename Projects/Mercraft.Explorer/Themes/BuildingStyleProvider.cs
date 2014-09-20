using System;
using System.Collections.Generic;
using Mercraft.Core.World.Buildings;
using Mercraft.Infrastructure.Primitives;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Themes
{
    /// <summary>
    ///     Provides the way to get BuildingStyle using Building
    /// </summary>
    public class BuildingStyleProvider : IBuildingStyleProvider
    {
        private readonly Dictionary<string, List<BuildingStyle>> _buildingTypeStyleMapping;

        public BuildingStyleProvider(Dictionary<string, List<BuildingStyle>> buildingTypeStyleMapping)
        {
            _buildingTypeStyleMapping = buildingTypeStyleMapping;
        }

        /// <summary>
        ///     Returns building style corresponding to given building 
        /// </summary>
        public BuildingStyle Get(Building building)
        {
            // NOTE we don't want to have osm specific logic in code outside osm project.
            // Tags mapping this should be done in mapcss file. So, we will throw exception 
            // here if type isn't mapped to existing style
            if (!_buildingTypeStyleMapping.ContainsKey(building.Type))
                throw new ArgumentException(String.Format(ErrorStrings.CannotGetBuildingStyle, building.Type));

            var styles = _buildingTypeStyleMapping[building.Type];

            if (styles.Count == 1)
                return styles[0];

            return FindBestMatch(building, styles);
        }

        private static BuildingStyle FindBestMatch(Building building, List<BuildingStyle> styles)
        {
            // TODO define additional data in BuildingStyle to improve matching

            var max = 0;
            var index = 0;
            for (int i = 0; i < styles.Count; i++)
            {
                var rating = Balance(building, styles[i]);
                if (rating > max)
                {
                    max = rating;
                    index = i;
                }
                else if (rating == max)
                {
                    // TODO should decide use new or keep old index to prevent "first win" strategy
                }
            }
            return styles[index];
        }

        private static int Balance(Building building, BuildingStyle style)
        {
            // NOTE different properties have different rating weight in range [1,5]
            // TODO rebalance this to have better matches
            var rating = 0;
            if (style.Floors == building.Levels)
                rating += 5;
            if (style.Facade.Material == building.FacadeMaterial)
                rating += 2;

            // Add color

            return rating;
        }
    }
}
