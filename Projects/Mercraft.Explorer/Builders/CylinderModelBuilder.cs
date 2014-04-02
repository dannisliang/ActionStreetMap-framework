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
            var height = rule.GetHeight(area);
            var minHeight = rule.GetMinHeight(area);
            return BuildCylinder(center, area.Points, height, minHeight);
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            // TODO is it applied to way?
            var height = rule.GetHeight(way);
            var minHeight = rule.GetMinHeight(way);
            return BuildCylinder(center, way.Points, height, minHeight);
        }

        private GameObject BuildCylinder(GeoCoordinate center, GeoCoordinate[] points, float height, float minHeight)
        {
            var circle = CircleHelper.GetCircle(center, points);
            var diameter = circle.Item1;
            var cylinderCenter = circle.Item2;

            var actualHeight = (height - minHeight) / 2;

            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.localScale = new Vector3(diameter, actualHeight, diameter);
            cylinder.transform.position = new Vector3(cylinderCenter.x, minHeight + actualHeight, cylinderCenter.y);

            return cylinder;
        }
    }
}
