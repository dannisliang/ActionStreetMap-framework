using System.Linq;
using Mercraft.Core.Elevation;
using Mercraft.Core.World.Buildings;
using Mercraft.Infrastructure.Dependencies;
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
        private readonly IResourceProvider _resourceProvider;

        [Dependency]
        public BuildingBuilder(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public void Build(HeightMap heightMap, Building building, BuildingStyle style)
        {
            // TODO select builders based on building id
            var facadeMeshData = style.Facade.Builders[0].Build(building, style);
            var roofMeshData = style.Roof.Builders[0].Build(building, style);

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
