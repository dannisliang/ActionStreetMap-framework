using System.Linq;
using Mercraft.Core.Elevation;
using Mercraft.Core.World.Buildings;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Unity;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public interface IBuildingBuilder
    {
        void Build(HeightMap heightMap, Building building, BuildingStyle style);
    }

    public class BuildingBuilder : IBuildingBuilder
    {
        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();
        private readonly IResourceProvider _resourceProvider;

        [Dependency]
        public BuildingBuilder(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public void Build(HeightMap heightMap, Building building, BuildingStyle style)
        {
            // NOTE we do not support elevations changes for buildings
            // should we use max point or min?
            building.Elevation = building.Footprint.Max(p => p.Elevation);

            for (int i = 0; i < building.Footprint.Length; i++)
                building.Footprint[i].Elevation = building.Elevation;

            if (!heightMap.IsFlat)
            {
                _heightMapProcessor.Recycle(heightMap);
                _heightMapProcessor.AdjustPolygon(building.Footprint, building.Elevation);
            }

            var facadeMeshData = style.Facade.Builder.Build(building, style);
            var roofMeshData = style.Roof.Builder.Build(building, style);

            var gameObject = building.GameObject.GetComponent<GameObject>();

            // NOTE use different gameObject to support different materials
            AttachChildGameObject(gameObject, "facade", facadeMeshData);
            AttachChildGameObject(gameObject, "roof", roofMeshData);
        }

        private void AttachChildGameObject(GameObject parent, string name, MeshData meshData)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.parent = parent.transform;

            var mesh = new Mesh();
            mesh.vertices = meshData.Vertices;
            mesh.triangles = meshData.Triangles;
            mesh.uv = meshData.UV;
            mesh.RecalculateNormals();

            var mf = gameObject.AddComponent<MeshFilter>();

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = _resourceProvider.GetMatertial(meshData.MaterialKey);
            renderer.material.mainTexture =  _resourceProvider.GetTexture(meshData.TextureKey);

            mf.mesh = mesh;
        }
    }
}
