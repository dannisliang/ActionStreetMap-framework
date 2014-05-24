using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Infrastructure;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class SphereModelBuilder: ModelBuilder
    {
        private readonly IGameObjectFactory _goFactory;

        public SphereModelBuilder(IGameObjectFactory goFactory)
        {
            _goFactory = goFactory;
        }

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildSphere(center, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            // TODO is it applied to way?
            return BuildSphere(center, way.Points, rule);
        }

        private IGameObject BuildSphere(GeoCoordinate center, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var sphereCenter = circle.Item2;

            IGameObject gameObjectWrapper = _goFactory.CreatePrimitive(PrimitiveType.Sphere);
            var sphere = gameObjectWrapper.GetComponent<GameObject>();

            sphere.AddComponent<MeshRenderer>();
            sphere.renderer.material = rule.GetMaterial();
            sphere.renderer.material.color = rule.GetFillColor();

            var minHeight = rule.GetMinHeight();
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.x, minHeight + diameter / 2, sphereCenter.y);

            return gameObjectWrapper;
        }
    }
}
