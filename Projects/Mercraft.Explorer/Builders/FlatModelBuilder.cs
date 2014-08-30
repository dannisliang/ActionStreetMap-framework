using System;
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
    public class FlatModelBuilder : ModelBuilder
    {
        private const int NoLayer = -1;
        public override string Name
        {
            get { return "flat"; }
        }

        [Dependency]
        public FlatModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(GeoCoordinate center, HeightMap heightMap, Rule rule, Area area)
        {
            base.BuildArea(center, heightMap, rule, area);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("{0} {1}", Name, area));
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            var floor = rule.GetZIndex();

            var verticies = PolygonHelper.GetVerticies2D(center, area.Points);

            var mesh = new Mesh();
            mesh.vertices = verticies.GetVerticies(floor);
            mesh.uv = verticies.GetUV();
            mesh.triangles = PolygonHelper.GetTriangles(verticies);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();

            gameObject.AddComponent<MeshRenderer>();
            gameObject.renderer.material = rule.GetMaterial();
            gameObject.renderer.material.color = rule.GetFillColor();

            var layerIndex = rule.GetLayerIndex(NoLayer);
            if (layerIndex != NoLayer)
                gameObject.layer = layerIndex;

            return gameObjectWrapper;
        }
    }
}