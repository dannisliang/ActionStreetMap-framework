using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings;
using Mercraft.Models.Buildings.Config;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class BuildingModelBuilder : ModelBuilder
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly TexturePackProvider _textureProvider;
        private readonly BuildingStyleProvider _styleProvider;

        [Dependency]
        public BuildingModelBuilder(IGameObjectFactory goFactory,
            TexturePackProvider textureProvider, BuildingStyleProvider styleProvider)
        {
            _goFactory = goFactory;
            _textureProvider = textureProvider;
            _styleProvider = styleProvider;
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
            return BuildBuilding(center, way,  way.Points, rule);
        }

        private IGameObject BuildBuilding(GeoCoordinate center, Model model, GeoCoordinate[] footPrint, Rule rule)
        {
            var gameObjectWrapper = _goFactory.CreateNew();
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            var verticies = PolygonHelper.GetVerticies2D(center, footPrint);
            var height = rule.GetHeight(NoValue);
            var levels = rule.GetLevels(NoValue);
            
            // TODO define theme somewhere
            var theme = "berlin";
            var styleName = rule.GetBuildingStyle();

            var style = _styleProvider.Get(theme, styleName);
            var texture = _textureProvider.Get(style.Texture);

            gameObject.AddComponent<BuildingBehavior>().Attach(RenderMode.Full, 
                new BuildingSettings()
                {
                    Seed = model.Id,
                    Height = height,
                    Levels = levels,
                    Style = style,
                    TexturePack = texture,
                    FootPrint = verticies
                });

            return gameObjectWrapper;
        }

    }
}
