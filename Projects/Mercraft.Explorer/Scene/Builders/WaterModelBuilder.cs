using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Models.Geometry;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    // NOTE this class has some duplicated in flat builder functionality
    public class WaterModelBuilder : ModelBuilder
    {
        private readonly IResourceProvider _resourceProvider;
        private const int NoLayer = -1;
        public override string Name
        {
            get { return "water"; }
        }

        [Dependency]
        public WaterModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory,
            IResourceProvider resourceProvider, IObjectPool objectPool)
            : base(worldManager ,gameObjectFactory, objectPool)
        {
            _resourceProvider = resourceProvider;
        }

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);

            if (WorldManager.Contains(area.Id))
                return null;

            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("{0} {1}", Name, area));

            var verticies2D = ObjectPool.NewList<MapPoint>();

            PointHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points, verticies2D);
            var offsetVerticies3D = GetOffsetPoints(verticies2D).GetVerticies(tile.HeightMap.MinElevation);
            var triangles = PointHelper.GetTriangles(verticies2D);
            WorldManager.AddModel(area.Id);

            ObjectPool.Store(verticies2D);

            BuildObject(gameObjectWrapper, rule, offsetVerticies3D, triangles);

            return gameObjectWrapper;
        }

        private MapPoint[] GetOffsetPoints(List<MapPoint> verticies)
        {
            // TODO determine value
            const float offset = -2f;
            var polygon = new Polygon(verticies);
            var result = new MapPoint[verticies.Count];
            for (int i = 0; i < polygon.Segments.Length; i++)
            {
                var previous = i == 0 ? polygon.Segments.Length - 1 : i - 1;

                var segment1 = polygon.Segments[previous];
                var segment2 = polygon.Segments[i];

                var parallel1 = SegmentUtils.GetParallel(segment1, offset);
                var parallel2 = SegmentUtils.GetParallel(segment2, offset);

                var ip1 = SegmentUtils.IntersectionPoint(parallel1, parallel2);

                result[i] = new MapPoint(ip1.x, ip1.z);
            }

            return result;
        }

        protected virtual void BuildObject(IGameObject gameObjectWrapper, Rule rule, Vector3[] points, int[] triangles)
        {
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();
            var mesh = new Mesh();
            mesh.vertices = points;
            //mesh.uv = verticies.GetUV();
            mesh.triangles = triangles;

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();

            gameObject.AddComponent<MeshRenderer>();
            gameObject.renderer.material = rule.GetMaterial(_resourceProvider);
            gameObject.renderer.material.color = rule.GetFillUnityColor();

            var layerIndex = rule.GetLayerIndex(NoLayer);
            if (layerIndex != NoLayer)
                gameObject.layer = layerIndex;
        }
    }
}
