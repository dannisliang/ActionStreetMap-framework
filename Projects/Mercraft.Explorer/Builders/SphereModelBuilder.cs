using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class SphereModelBuilder : ModelBuilder
    {
        [Dependency]
        public SphereModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildSphere(center, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            // TODO is it applied to way?
            return BuildSphere(center, way, way.Points, rule);
        }

        private IGameObject BuildSphere(GeoCoordinate center, Model model, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var sphereCenter = circle.Item2;

            IGameObject gameObjectWrapper = GameObjectFactory.CreatePrimitive(String.Format("Spfere {0}", model),
                UnityPrimitiveType.Sphere);
            var sphere = gameObjectWrapper.GetComponent<GameObject>();

            sphere.AddComponent<MeshRenderer>();
            sphere.renderer.material = rule.GetMaterial();
            sphere.renderer.material.color = rule.GetFillColor();

            var minHeight = rule.GetMinHeight();
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.X, minHeight + diameter/2, sphereCenter.Y);

            return gameObjectWrapper;
        }
    }
}