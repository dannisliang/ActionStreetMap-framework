using System;
using System.Collections.Generic;
using Mercraft.Core.World.Buildings;
using Mercraft.Core.World.Infos;
using Mercraft.Core.World.Roads;
using Mercraft.Models.Buildings;
using Mercraft.Models.Infos;
using Mercraft.Models.Roads;

namespace Mercraft.Explorer.Themes
{
    public class Theme
    {
        private readonly IBuildingStyleProvider _buildingStyleProvider;
        private readonly IRoadStyleProvider _roadStyleProvider;
        private readonly IInfoStyleProvider _infoStyleProvider;
        private readonly Dictionary<Type, object> _providers;

        public string Name { get; set; }

        public Theme(IBuildingStyleProvider buildingStyleProvider, IRoadStyleProvider roadStyleProvider, 
            IInfoStyleProvider infoStyleProvider)
        {
            _buildingStyleProvider = buildingStyleProvider;
            _roadStyleProvider = roadStyleProvider;
            _infoStyleProvider = infoStyleProvider;
            _providers = new Dictionary<Type, object>
            {
                {typeof (IBuildingStyleProvider), _buildingStyleProvider},
                {typeof (IRoadStyleProvider), _roadStyleProvider},
                {typeof (IInfoStyleProvider), _infoStyleProvider}
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

        public InfoStyle GetInfoStyle(Info info)
        {
            return _infoStyleProvider.Get(info);
        }

        public T GetStyleProvider<T>() where T:class
        {
            return (T) _providers[typeof(T)];
        }
    }
}
