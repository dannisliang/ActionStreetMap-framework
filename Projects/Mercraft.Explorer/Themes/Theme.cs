using System;
using System.Collections.Generic;
using System.Reflection;
using Mercraft.Core.World.Buildings;
using Mercraft.Core.World.Roads;
using Mercraft.Models.Buildings;
using Mercraft.Models.Roads;

namespace Mercraft.Explorer.Themes
{
    public class Theme
    {
        private readonly IBuildingStyleProvider _buildingStyleProvider;
        private readonly IRoadStyleProvider _roadStyleProvider;
        private readonly Dictionary<Type, object> _providers;

        public string Name { get; set; }

        public Theme(IBuildingStyleProvider buildingStyleProvider, IRoadStyleProvider roadStyleProvider)
        {
            _buildingStyleProvider = buildingStyleProvider;
            _roadStyleProvider = roadStyleProvider;
            _providers = new Dictionary<Type, object>()
            {
                {typeof (IBuildingStyleProvider), _buildingStyleProvider},
                {typeof (IRoadStyleProvider), _roadStyleProvider},
            };
        }

        public BuildingStyle GetBuildingStyle(Building building)
        {
            return _buildingStyleProvider.Get(building);
        }

        public RoadStyle GetRoadStyle(Road road)
        {
            return _roadStyleProvider.Get(road);
        }

        public T GetStyleProvider<T>() where T:class
        {
            return (T) _providers[typeof(T)];
        }
    }
}
