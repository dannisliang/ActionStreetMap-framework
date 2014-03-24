using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class FlatModelBuilder : ModelBuilder
    {
        public override void BuildArea(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
        {
            // TODO remove this assertion after handling this case above
            if (area.Points.Length < 3)
            {
                Debug.LogError("Area contains less than 3 points: " + area);
                return;
            }

            gameObject.name = String.Format("Flat {0}", area);

            var floor = rule.GetZIndex(area);

            var verticies = PolygonHelper.GetVerticies2D(center, area.Points);

            var mesh = new Mesh();
            mesh.vertices = PolygonHelper.GetVerticies(verticies, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles(verticies);

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();
        }

        public override void BuildWay(GeoCoordinate center, GameObject gameObject, Rule rule, Way way)
        {
            var zIndex = rule.GetZIndex(way);

            gameObject.name = String.Format("Flat {0}", way);

            var points = PolygonHelper.GetVerticies2D(center, way.Points);

            var lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = rule.GetMaterial(way);
            lineRenderer.material.color = rule.GetFillColor(way, Color.red);
            lineRenderer.SetVertexCount(points.Length);


            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(points[i].x, zIndex, points[i].y));
            }

            var width = rule.GetWidth(way);
            lineRenderer.SetWidth(width, width);
        }
    }
}
