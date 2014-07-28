using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Buildings;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Buildings;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private readonly IBuildingBuilder _builder;
        //private readonly IBuildingStyleProvider _styleProvider;

        [Dependency]
        public BuildingModelBuilder(IGameObjectFactory goFactory, 
            IBuildingBuilder builder):
            //IBuildingStyleProvider styleProvider):
            base(goFactory)
        {
            _builder = builder;
            //_styleProvider = styleProvider;
        }

        private const int NoValue = 0;

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildBuilding(center, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            return BuildBuilding(center, way, way.Points, rule);
        }

        private IGameObject BuildBuilding(GeoCoordinate center, Model model, GeoCoordinate[] footPrint, Rule rule)
        {
            var gameObjectWrapper = _goFactory.CreateNew(String.Format("Building {0}", model));
            
            // TODO should we save this object in WorldManager?
            var building = new Building()
            {
                Address = AddressExtractor.Extract(model.Tags),
                GameObject = gameObjectWrapper,
                Height = rule.GetHeight(NoValue),
                Levels = rule.GetLevels(NoValue),
                Type = rule.GetBuildingType(),
                BottomOffset = rule.GetZIndex(),
                Footprint = PolygonHelper.GetVerticies2D(center, footPrint)
            };

            //var style = _styleProvider.Get(building);

            BuildingStyle style = null;

            _builder.Build(building, style);

            return gameObjectWrapper;
        }
    }
}