using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Geometry;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    /// <summary>
    ///     Provides logic to build water.
    /// </summary>
    public class WaterModelBuilder : ModelBuilder
    {
        private readonly ITerrainBuilder _terrainBuilder;
        private const int NoLayer = -1;

        /// <inheritdoc />
        public override string Name
        {
            get { return "water"; }
        }

        /// <summary>
        ///     Creates WaterModelBuilder.
        /// </summary>
        [Dependency]
        public WaterModelBuilder(ITerrainBuilder terrainBuilder)
        {
            _terrainBuilder = terrainBuilder;
        }

        /// <inheritdoc />
        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);

            if (WorldManager.Contains(area.Id))
                return null;

            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("{0} {1}", Name, area));

            var verticies2D = ObjectPool.NewList<MapPoint>();

            PointHelper.GetPolygonPoints(tile.HeightMap, tile.RelativeNullPoint, area.Points, verticies2D);
            var offsetPoints = GetOffsetPoints(verticies2D);

            // NOTE we should subtract some value from min elevation
            // but it's better to make this value confgurable
            var elevation = verticies2D.Min(v => v.Elevation);
            _terrainBuilder.AddElevation(new AreaSettings
            {
                ZIndex = rule.GetZIndex(),
                Elevation = elevation - 3, 
                Points = offsetPoints
            });

            var offsetVerticies3D = offsetPoints.GetVerticies(elevation - 1f);
            var triangles = PointHelper.GetTriangles(verticies2D);
            WorldManager.AddModel(area.Id);

            ObjectPool.Store(verticies2D);

            BuildObject(gameObjectWrapper, rule, offsetVerticies3D, triangles);

            return gameObjectWrapper;
        }

        private List<MapPoint> GetOffsetPoints(List<MapPoint> verticies)
        {
            // TODO determine the best value
            const float offset = -2f;
            var polygon = new Polygon(verticies);
            var result = ObjectPool.NewList<MapPoint>(verticies.Count);
            for (int i = 0; i < polygon.Segments.Length; i++)
            {
                var previous = i == 0 ? polygon.Segments.Length - 1 : i - 1;

                var segment1 = polygon.Segments[previous];
                var segment2 = polygon.Segments[i];

                var parallel1 = SegmentUtils.GetParallel(segment1, offset);
                var parallel2 = SegmentUtils.GetParallel(segment2, offset);

                var ip1 = SegmentUtils.IntersectionPoint(parallel1, parallel2);

                result.Add(new MapPoint(ip1.x, ip1.z));
            }

            return result;
        }

        /// <summary>
        ///     Process unity specific data.
        /// </summary>
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
            gameObject.renderer.material = rule.GetMaterial(ResourceProvider);
            gameObject.renderer.material.color = rule.GetFillUnityColor();

            var layerIndex = rule.GetLayerIndex(NoLayer);
            if (layerIndex != NoLayer)
                gameObject.layer = layerIndex;
        }
    }
}
