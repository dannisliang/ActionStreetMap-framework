using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Themes
{
    /// <summary>
    ///     Provides the way to get BuildingStyle using Building
    /// </summary>
    public class BuildingStyleProvider : IBuildingStyleProvider
    {
        private readonly Dictionary<string, List<BuildingStyle.FacadeStyle>> _facadeStyleMapping;
        private readonly Dictionary<string, List<BuildingStyle.RoofStyle>> _roofStyleMapping;

        public BuildingStyleProvider(Dictionary<string, List<BuildingStyle.FacadeStyle>> facadeStyleMapping,
            Dictionary<string, List<BuildingStyle.RoofStyle>> roofStyleMapping)
        {
            _facadeStyleMapping = facadeStyleMapping;
            _roofStyleMapping = roofStyleMapping;
        }

        /// <summary>
        ///     Returns building style corresponding to given building 
        /// </summary>
        public BuildingStyle Get(Building building)
        {
            // NOTE we don't want to have osm specific logic in code outside osm project.
            // Tags mapping this should be done in mapcss file. So, we will throw exception 
            // here if type isn't mapped to existing style
            if (!_facadeStyleMapping.ContainsKey(building.Type))
                throw new ArgumentException(String.Format(ErrorStrings.CannotGetBuildingStyle, building.Type));

            var facadeStyle = _facadeStyleMapping[building.Type];
            var roofStyle = _roofStyleMapping[building.Type];

            if (facadeStyle.Count == 1 && roofStyle.Count == 1)
                return new BuildingStyle()
                {
                    Facade = facadeStyle.First(),
                    Roof = roofStyle.First()
                };

            return FindBestMatch(building, facadeStyle, roofStyle);
        }

        private static BuildingStyle FindBestMatch(Building building, List<BuildingStyle.FacadeStyle> facadeStyles,
            List<BuildingStyle.RoofStyle> roofStyles)
        {
            // TODO define additional data in BuildingStyle to improve matching

            var max = 0;
            var index = 0;
            for (int i = 0; i < facadeStyles.Count; i++)
            {
                var rating = Balance(building, facadeStyles[i]);
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
            return new BuildingStyle()
            {
                Facade = facadeStyles[index],
                Roof = roofStyles.First()
            };
        }

        private static int Balance(Building building, BuildingStyle.FacadeStyle style)
        {
            // NOTE different properties have different rating weight in range [1,5]
            // TODO rebalance this to have better matches
            var rating = 0;
            if (style.Floors == building.Levels)
                rating += 5;
            if (style.Material == building.FacadeMaterial)
                rating += 2;

            // Add color

            return rating;
        }
    }
}
