﻿using Mercraft.Core.World.Buildings;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Themes
{
    public interface IBuildingStyleProvider
    {
        BuildingStyle Get(Theme theme, Building building);
    }

    public class BuildingStyleProvider : IBuildingStyleProvider
    {
        public BuildingStyle Get(Theme theme, Building building)
        {
            // TODO use smart logic to choose building style
            return theme.BuildingTypeStyleMapping[building.Type][0];
        }
    }
}
