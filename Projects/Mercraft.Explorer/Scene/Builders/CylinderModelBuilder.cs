using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
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

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);
            return BuildCylinder(tile.RelativeNullPoint, area, area.Points, rule);
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            base.BuildWay(tile, rule, way);
            // TODO is it applied to way?
            return BuildCylinder(tile.RelativeNullPoint, way, way.Points, rule);
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