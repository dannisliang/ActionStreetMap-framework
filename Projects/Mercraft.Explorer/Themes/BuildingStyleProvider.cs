using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;
using Mercraft.Models.Utils;

namespace Mercraft.Explorer.Themes
{
    /// <summary>
    ///     Provides the way to get BuildingStyle using Building
    /// </summary>
    public class BuildingStyleProvider : IBuildingStyleProvider
    {
        private readonly Dictionary<string, List<BuildingStyle.FacadeStyle>> _facadeStyleMapping;
        private readonly Dictionary<string, List<BuildingStyle.RoofStyle>> _roofStyleMapping;

        private readonly List<int> _matchedIndicies = new List<int>(16);

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
                return new BuildingStyle
                {
                    Facade = facadeStyle.First(),
                    Roof = roofStyle.First()
                };

            return FindBestMatch(building, facadeStyle, roofStyle);
        }

        private BuildingStyle FindBestMatch(Building building, List<BuildingStyle.FacadeStyle> facadeStyles,
            List<BuildingStyle.RoofStyle> roofStyles)
        {
            // TODO define additional data in BuildingStyle to improve matching

            var max = -1;
            for (int i = 0; i < facadeStyles.Count; i++)
            {
                var rating = BalanceFacade(building, facadeStyles[i]);
                // OR branch allows to avoid first-win strategy 
                if (rating > max)
                {
                    max = rating;
                    _matchedIndicies.Clear();
                    _matchedIndicies.Add(i);
                }
                else if (rating == max)
                {
                    _matchedIndicies.Add(i);
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

            var facadeIndex = _matchedIndicies[RandomHelper.GetIndex(building.Id, _matchedIndicies.Count)];

            return new BuildingStyle
            {
                Facade = facadeStyles[facadeIndex],
                Roof = roofStyles[roofIndex]
            };
        }

        private int BalanceFacade(Building building, BuildingStyle.FacadeStyle style)
        {
            // NOTE different properties have different rating weight in range [1,5]
            // TODO rebalance this to have better matches
            var rating = 0;
            if (style.Height == building.Levels)
                rating += 3;
            if (building.FacadeMaterial != null && style.Material == building.FacadeMaterial)
                rating += 3;
            if (style.Color.Equals(building.FacadeColor))
                rating += 2;

            return rating;
        }

        private int BalanceRoof(Building building, BuildingStyle.RoofStyle style)
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
