
using System;
using System.Collections;
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
    public class SolidModelBuilder : ModelBuilder
    {
        public override GameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            GameObject gameObject = new GameObject();

            gameObject.name = String.Format("Solid {0}", area);
            BuildModel(center, gameObject, rule, area, area.Points.ToList());
            return gameObject;
        }

        public override GameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            GameObject gameObject = new GameObject();

            gameObject.name = String.Format("Solid {0}", way);
            BuildModel(center, gameObject, rule, way, way.Points.ToList());

            return gameObject;
        }

        private void BuildModel(GeoCoordinate center, GameObject gameObject, Rule rule, Model model, IList<GeoCoordinate> coordinates)
        {
            var height = rule.GetHeight(model);

            var floor = rule.GetZIndex(model);
            var top = floor + height;

            var verticies = PolygonHelper.GetVerticies2D(center, coordinates);

            var mesh = new Mesh();
            mesh.vertices = PolygonHelper.GetVerticies3D(verticies, top, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles3D(verticies);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();

            gameObject.AddComponent<MeshRenderer>();
            gameObject.renderer.material = rule.GetMaterial(model);
            gameObject.renderer.material.color = rule.GetFillColor(model);
        }
    }
}
