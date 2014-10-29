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
using Mercraft.Models.Geometry.Polygons;
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

            var verticies2D = ObjectPool.NewList<MapPoint>();

            // get polygon map points
            PointHelper.GetPolygonPoints(tile.HeightMap, tile.RelativeNullPoint, area.Points, verticies2D);

            // detect minimal elevation for water
            var elevation = verticies2D.Min(v => v.Elevation);

            // cut polygon by current tile
            PolygonUtils.ClipPolygonByTile(tile.BottomLeft, tile.TopRight, verticies2D);

            // get offset points to prevent gaps between water polygon and terrain due to issues 
            // on low terrain heightmap resolutions
            // NOTE current polygon cut algorithm may produce self-intersection results
            // TODO have to test current offset alhorithm
            // TODO determine better offset constant or make it configurable
            var offsetPoints = ObjectPool.NewList<MapPoint>(verticies2D.Count);
            PolygonUtils.MakeOffset(verticies2D, offsetPoints, -2f);
      
            // add elevation
            _terrainBuilder.AddElevation(new AreaSettings
            {
                ZIndex = rule.GetZIndex(),
                Elevation = elevation - 3, 
                Points = offsetPoints
            });

            var vector3Ds = offsetPoints.GetVerticies(elevation - 1f);
            var triangles = PointHelper.GetTriangles(offsetPoints);

            ObjectPool.Store(verticies2D);

            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("{0} {1}", Name, area));
            BuildObject(gameObjectWrapper, rule, vector3Ds, triangles);

            return gameObjectWrapper;
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
