
using System;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.Builders.Areas
{
    public class SolidAreaBuilder : ModelBuilder
    {
        public override void BuildArea(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
        {
            gameObject.name = String.Format("Solid area {0}", area.Id);

            var height = rule.GetHeight(area);
            var floor = rule.GetZIndex(area); ;
            var top = floor + height;

            var verticies = PolygonHelper.GetVerticies2D(center, area.Points.ToList());

            var mesh = new Mesh();
            mesh.vertices = PolygonHelper.GetVerticies3D(verticies, top, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles(verticies);

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            //meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.RecalculateNormals();
            //meshFilter.mesh.Optimize();
        }
    }
}
