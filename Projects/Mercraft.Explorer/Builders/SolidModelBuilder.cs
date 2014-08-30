using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
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
    public class SolidModelBuilder : ModelBuilder
    {
        public override string Name
        {
            get { return "solid"; }
        }

        [Dependency]
        public SolidModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, HeightMap heightMap, Rule rule, Area area)
        {
            base.BuildArea(center, heightMap, rule, area);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Solid {0}", area));
            BuildModel(center, gameObjectWrapper, rule, area.Points.ToList());
            return gameObjectWrapper;
        }

        public override IGameObject BuildWay(GeoCoordinate center, HeightMap heightMap, Rule rule, Way way)
        {
            base.BuildWay(center, heightMap, rule, way);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Solid {0}", way));
            BuildModel(center, gameObjectWrapper, rule, way.Points.ToList());
            return gameObjectWrapper;
        }

        private void BuildModel(GeoCoordinate center, IGameObject gameObject, Rule rule,
            IList<GeoCoordinate> coordinates)
        {
            var height = rule.GetHeight();

            var floor = rule.GetZIndex();
            var top = floor + height;

            var verticies = PolygonHelper.GetVerticies2D(center, coordinates);

            var mesh = new Mesh();
            mesh.vertices = verticies.GetVerticies3D(top, floor);
            mesh.uv = verticies.GetUV();
            mesh.triangles = PolygonHelper.GetTriangles3D(verticies);

            var unityGameObject = gameObject.GetComponent<GameObject>();

            var meshFilter = unityGameObject.AddComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();

            unityGameObject.AddComponent<MeshRenderer>();
            unityGameObject.renderer.material = rule.GetMaterial();
            unityGameObject.renderer.material.color = rule.GetFillColor();
        }
    }
}