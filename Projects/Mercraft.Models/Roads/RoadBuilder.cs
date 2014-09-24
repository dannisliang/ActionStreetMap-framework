using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Elevation;
using Mercraft.Core.World.Roads;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Utils;
using Mercraft.Models.Utils.Lines;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    /// <summary>
    ///     Builds road using road model
    /// </summary>
    public interface IRoadBuilder
    {
        void Build(HeightMap heightMap, Road road, RoadStyle style);
    }

    public class RoadBuilder : IRoadBuilder
    {
        private readonly IResourceProvider _resourceProvider;
        private readonly ThickLineBuilder _lineBuilder = new ThickLineBuilder();

        [Dependency]
        public RoadBuilder(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public void Build(HeightMap heightMap, Road road, RoadStyle style)
        {
            var lineElements = road.Elements.Select(e => new LineElement<RoadElement>(e, e.Points, e.Width)).ToArray();
            _lineBuilder.Build(heightMap, lineElements, (p, t, u) => CreateMesh(road, style, p, t, u));
        }

        protected virtual void CreateMesh(Road road, RoadStyle style,
            List<Vector3> points, List<int> triangles, List<Vector2> uv)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = points.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();

            var gameObject = road.GameObject.GetComponent<GameObject>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<RoadBehavior>().Road = road;
            gameObject.tag = "osm.road";

            var renderer = gameObject.AddComponent<MeshRenderer>();

            renderer.material = _resourceProvider.GetMatertial(style.Materials[RandomHelper
                .GetIndex(road.Elements.First().Id, style.Materials.Length)]);

            renderer.material.mainTexture = _resourceProvider.GetTexture(style.Textures[RandomHelper
                .GetIndex(road.Elements.First().Id, style.Textures.Length)]);
        }

    }
}
