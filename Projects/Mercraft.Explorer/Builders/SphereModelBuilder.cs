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
            //var height = rule.GetHeight(area);
            var minHeight = rule.GetMinHeight(area);
            return BuildSphere(center, area.Points, minHeight);
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            // TODO is it applied to way?
            var minHeight = rule.GetMinHeight(way);
            return BuildSphere(center, way.Points, minHeight);
        }

        private GameObject BuildSphere(GeoCoordinate center, GeoCoordinate[] points, float minHeight)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var sphereCenter = circle.Item2;

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
            sphere.transform.position = new Vector3(sphereCenter.x, minHeight + diameter / 2, sphereCenter.y);

            return sphere;
        }
    }
}
