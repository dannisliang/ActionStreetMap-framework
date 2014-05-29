using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class CylinderModelBuilder : ModelBuilder
    {
        private readonly IGameObjectFactory _goFactory;

        [Dependency]
        public CylinderModelBuilder(IGameObjectFactory goFactory)
        {
            _goFactory = goFactory;
        }

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildCylinder(center, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            // TODO is it applied to way?
            return BuildCylinder(center, way, way.Points, rule);
        }

        private IGameObject BuildCylinder(GeoCoordinate center, Model model, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var cylinderCenter = circle.Item2;

            var height = rule.GetHeight();
            var minHeight = rule.GetMinHeight();

            var actualHeight = (height - minHeight)/2;

            var gameObjectWrapper = _goFactory.CreatePrimitive(String.Format("Cylinder {0}", model), 
                PrimitiveType.Cylinder);
            var cylinder = gameObjectWrapper.GetComponent<GameObject>();

            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.x, minHeight + actualHeight, cylinderCenter.y);


            cylinder.AddComponent<MeshRenderer>();
            cylinder.renderer.material = rule.GetMaterial();
            cylinder.renderer.material.color = rule.GetFillColor();

            return gameObjectWrapper;
        }
    }
}