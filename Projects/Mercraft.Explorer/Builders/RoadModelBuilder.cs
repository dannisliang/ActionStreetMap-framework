using System;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Roads;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class RoadModelBuilder : ModelBuilder
    {
        [Dependency]
        public RoadModelBuilder(IGameObjectFactory goFactory) : base(goFactory)
        {
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            IGameObject gameObjectWrapper = _goFactory.CreateNew(String.Format("Road {0}", way));
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            var zIndex = rule.GetZIndex();
            var points = way.Points
                .Select(g => GeoProjection.ToMapCoordinate(center, g))
                .Select(p => new Vector3(p.X, zIndex, p.Y))
                .ToArray();

            gameObject.AddComponent<RoadBehavior>().Attach(new RoadSettings()
            {
                Points = points,
                Height = 0.1f,
                Width = rule.GetWidth(),
                Material = rule.GetMaterial(),
                Color =   rule.GetFillColor()
            });

            return gameObjectWrapper;
        }
    }
}