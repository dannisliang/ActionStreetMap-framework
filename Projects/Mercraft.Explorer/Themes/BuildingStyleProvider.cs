using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;
using Mercraft.Models.Utils;
using UnityEngine;
using Color32 = Mercraft.Core.Unity.Color32;

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
            // NOTE So far, this matcher uses only color to find the best style
            // as default implementation assumes that we're using only RAL colored textures
            // no real brick material, facades, etc
            const int threshold = 5;

            // facade
            var facadeIndex = 0;
            var currentDiff = int.MaxValue;
            for (int i = 0; i < facadeStyles.Count; i++)
            {
                var difference = CalcColorDifference(building.FacadeColor, facadeStyles[i].Color);
                if (difference < currentDiff)
                {
                    currentDiff = difference;
                    facadeIndex = i;
                    if (currentDiff <= threshold) break;
                }
            }

            // roof
            var roofIndex = 0;
            currentDiff = int.MaxValue;
            for (int i = 0; i < roofStyles.Count; i++)
            {
                var difference = CalcColorDifference(building.RoofColor, roofStyles[i].Color);
                if (difference < currentDiff)
                {
                    currentDiff = difference;
                    roofIndex = i;
                    if (currentDiff <= threshold) break;
                }
            }

            return new BuildingStyle
            {
                Facade = facadeStyles[facadeIndex],
                Roof = roofStyles[roofIndex]
            };
        }

        private int CalcColorDifference(Color32 first, Color32 second)
        {
            return Math.Abs(first.R - second.R) +
                   Math.Abs(first.G - second.G) +
                   Math.Abs(first.B - second.B);
        }
    }
}
