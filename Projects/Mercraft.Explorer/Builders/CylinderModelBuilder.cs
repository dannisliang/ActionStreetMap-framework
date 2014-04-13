using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class CylinderModelBuilder: ModelBuilder
    {
        public override GameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildCylinder(center, area.Points, rule);
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            // TODO is it applied to way?
            return BuildCylinder(center, way.Points, rule);
        }

        private GameObject BuildCylinder(GeoCoordinate center, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var cylinderCenter = circle.Item2;

            var height = rule.GetHeight();
            var minHeight = rule.GetMinHeight();

            var actualHeight = (height - minHeight) / 2;

            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.x, minHeight + actualHeight, cylinderCenter.y);


            cylinder.AddComponent<MeshRenderer>();
            cylinder.renderer.material = rule.GetMaterial();
            cylinder.renderer.material.color = rule.GetFillColor();

            return cylinder;
        }
    }
}
