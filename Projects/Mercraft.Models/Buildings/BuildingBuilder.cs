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
            var facadeMeshData = style.Facade.Builders[RandomHelper
                .GetIndex(building.Id, style.Facade.Builders.Length)]
                .Build(building, style);

            var roofMeshData = style.Roof.Builders[RandomHelper
                .GetIndex(building.Id, style.Roof.Builders.Length)]
                .Build(building, style);

            var gameObject = building.GameObject.GetComponent<GameObject>();

            // NOTE use different gameObject only to support different materials
            AttachChildGameObject(gameObject, "facade", facadeMeshData, 
                style.Desc.AllowSetColor? UnityColorUtility.FromCore(building.FacadeColor): default(Color32));
            AttachChildGameObject(gameObject, "roof", roofMeshData, default(Color32));
        }

        private void AttachChildGameObject(GameObject parent, string name, MeshData meshData, Color32 color)
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

            if (!UnityColorUtility.IsDefault(color))
                renderer.material.color = color;

            mf.mesh = mesh;
        }
    }
}
