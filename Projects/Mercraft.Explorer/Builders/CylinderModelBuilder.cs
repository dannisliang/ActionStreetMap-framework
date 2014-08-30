using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class CylinderModelBuilder : ModelBuilder
    {
        public override string Name
        {
            get { return "cylinder"; }
        }

        [Dependency]
        public CylinderModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, HeightMap heightMap, Rule rule, Area area)
        {
            base.BuildArea(center, heightMap, rule, area);
            return BuildCylinder(center, area, area.Points, rule);
        }

        public override IGameObject BuildWay(GeoCoordinate center, HeightMap heightMap, Rule rule, Way way)
        {
            base.BuildWay(center, heightMap, rule, way);
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

            var gameObjectWrapper = GameObjectFactory.CreatePrimitive(String.Format("Cylinder {0}", model),
                UnityPrimitiveType.Cylinder);
            var cylinder = gameObjectWrapper.GetComponent<GameObject>();

            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.X, minHeight + actualHeight, cylinderCenter.Y);


            cylinder.AddComponent<MeshRenderer>();
            cylinder.renderer.material = rule.GetMaterial();
            cylinder.renderer.material.color = rule.GetFillColor();

            return gameObjectWrapper;
        }
    }
}