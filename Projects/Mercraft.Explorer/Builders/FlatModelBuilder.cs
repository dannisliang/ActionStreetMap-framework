using System;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class FlatModelBuilder : ModelBuilder
    {
        private readonly IGameObjectFactory _goFactory;

        [Dependency]
        public FlatModelBuilder(IGameObjectFactory goFactory)
        {
            _goFactory = goFactory;
        }

        public override IGameObject BuildArea(GeoCoordinate center, Rule rule, Area area)
        {
            base.BuildArea(center, rule, area);
            IGameObject gameObjectWrapper = _goFactory.CreateNew();
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            gameObject.name = String.Format("Flat {0}", area);

            var floor = rule.GetZIndex();

            var verticies = PolygonHelper.GetVerticies2D(center, area.Points);

            var mesh = new Mesh();
            mesh.vertices = PolygonHelper.GetVerticies(verticies, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles(verticies);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();

            gameObject.AddComponent<MeshRenderer>();
            gameObject.renderer.material = rule.GetMaterial();
            gameObject.renderer.material.color = rule.GetFillColor();

            return gameObjectWrapper;
        }

        public override IGameObject BuildWay(GeoCoordinate center, Rule rule, Way way)
        {
            base.BuildWay(center, rule, way);
            IGameObject gameObjectWrapper = _goFactory.CreateNew();
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();
            var zIndex = rule.GetZIndex();

            gameObject.name = String.Format("Flat {0}", way);

            var points = PolygonHelper.GetVerticies2D(center, way.Points);

            var lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = rule.GetMaterial();
            lineRenderer.material.color = rule.GetFillColor();
            lineRenderer.SetVertexCount(points.Length);


            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(points[i].x, zIndex, points[i].y));
            }

            var width = rule.GetWidth();
            lineRenderer.SetWidth(width, width);

            return gameObjectWrapper;
        }
    }
}
