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
    public class SphereModelBuilder: ModelBuilder
    {
        public override GameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            return BuildSphere(center, area.Points, rule);
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            // TODO is it applied to way?
            return BuildSphere(center, way.Points, rule);
        }

        private GameObject BuildSphere(GeoCoordinate center, GeoCoordinate[] points, Rule rule)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var sphereCenter = circle.Item2;

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            sphere.AddComponent<MeshRenderer>();
            sphere.renderer.material = rule.GetMaterial();
            sphere.renderer.material.color = rule.GetFillColor();

            var minHeight = rule.GetMinHeight();
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.x, minHeight + diameter / 2, sphereCenter.y);

            return sphere;
        }
    }
}
