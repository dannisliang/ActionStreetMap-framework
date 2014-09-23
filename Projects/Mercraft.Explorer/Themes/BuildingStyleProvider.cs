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
            var facadeIndex = 0;
            for (int i = 0; i < facadeStyles.Count; i++)
            {
                var rating = BalanceFacade(building, facadeStyles[i]);
                // OR branch allows to avoid first-win strategy 
                if (rating > max || (rating != 0 && rating == max && building.Id % 2 == 0))
                {
                    max = rating;
                    facadeIndex = i;
                }
            }

            max = 0;
            var roofIndex = 0;
            for (int i = 0; i < roofStyles.Count; i++)
            {
                var rating = BalanceRoof(building, roofStyles[i]);
                // OR branch allows to avoid first-win strategy 
                if (rating > max || (rating != 0 && rating == max && building.Id % 2 == 0))
                {
                    max = rating;
                    roofIndex = i;
                }
            }

            return new BuildingStyle()
            {
                Facade = facadeStyles[facadeIndex],
                Roof = roofStyles[roofIndex]
            };
        }

        private static int BalanceFacade(Building building, BuildingStyle.FacadeStyle style)
        {
            // NOTE different properties have different rating weight in range [1,5]
            // TODO rebalance this to have better matches
            var rating = 0;
            if (style.Floors == building.Levels)
                rating += 3;
            if (building.FacadeMaterial != null && style.Material == building.FacadeMaterial)
                rating += 3;
            if (style.Color.Equals(building.FacadeColor))
                rating += 3;

            return rating;
        }

        private static int BalanceRoof(Building building, BuildingStyle.RoofStyle style)
        {
            // NOTE different properties have different rating weight in range [1,5]
            // TODO rebalance this to have better matches
            var rating = 0;
            if (building.RoofType != null && style.Type == building.RoofType)
                rating += 5;
            if (building.RoofMaterial != null && style.Material == building.RoofMaterial)
                rating += 3;
            if (style.Color.Equals(building.RoofColor))
                rating += 3;

            return rating;
        }
    }
}
