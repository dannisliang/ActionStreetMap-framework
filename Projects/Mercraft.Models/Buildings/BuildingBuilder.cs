using System;
using System.Linq;
using Mercraft.Core.Elevation;
using Mercraft.Core.World.Buildings;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Buildings.Roofs;
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

            var roofMeshData = GetRoofBuilder(building, style.Roof.Builders)
               .Build(building, style);

            var gameObject = building.GameObject.GetComponent<GameObject>();

            // NOTE use different gameObject only to support different materials
            AttachChildGameObject(gameObject, "facade", facadeMeshData, 
                style.Facade.AllowSetColor? UnityColorUtility.FromCore(building.FacadeColor): default(Color32));
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

        private IRoofBuilder GetRoofBuilder(Building building, IRoofBuilder[] roofBuildings)
        {
            // for most of buildings, roof type isn't defined, but we want to use different types
            // however, we have to check whether we can build roof using random roof builder

            // contains true for index of roof builder if it can build roof for given building
            bool[] ids = new bool[roofBuildings.Length];

            int count = 0;
            for (int i = 0; i < roofBuildings.Length; i++)
            {
                if (roofBuildings[i].CanBuild(building))
                {
                    ids[i] = true;
                    count++;
                }
            }

            if (count == 0)
                throw new InvalidOperationException("Cannot find roof builder which can build roof of given building - suspect wrong theme definition");

            // however, we don't want to use first occurrence, use building Id as seed
            int index = 0;
            var seed = building.Id % count;
            while (true)
            {
                if (ids[index] && index == seed)
                    return roofBuildings[index];
                index++;
            }
        }
    }
}
