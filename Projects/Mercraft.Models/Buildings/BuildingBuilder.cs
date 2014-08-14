using Mercraft.Core.World.Buildings;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public interface IBuildingBuilder
    {
        void Build(Building building, BuildingStyle style);
    }

    public class BuildingBuilder : IBuildingBuilder
    {
        public void Build(Building building, BuildingStyle style)
        {
            var facadeMeshData = style.Facade.Builder.Build(building, style);
            var roofMeshData = style.Roof.Builder.Build(building, style);

            // roof triangles calculated starting from 0, so we need to add offset
            var trisOffset = facadeMeshData.Vertices.Length;
            for (int i = 0; i < roofMeshData.Triangles.Length; i++)
                roofMeshData.Triangles[i] += trisOffset;

            var gameObject = building.GameObject.GetComponent<GameObject>();
            
            var mesh = new Mesh();
            mesh.vertices = Merge(facadeMeshData.Vertices, roofMeshData.Vertices);
            mesh.triangles = Merge(facadeMeshData.Triangles, roofMeshData.Triangles);
            mesh.uv = Merge(facadeMeshData.UV, roofMeshData.UV);
            mesh.RecalculateNormals();

            var mf = gameObject.AddComponent<MeshFilter>();

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = Resources.Load<Material>(style.Facade.Material);
            renderer.material.mainTexture = Resources.Load<Texture>(style.Facade.Texture);

            mf.mesh = mesh;

            gameObject.AddComponent<MeshCollider>();
        }

        private T[] Merge<T>(T[] source1, T[] source2)
        {
            T[] result = new T[source1.Length + source2.Length];
            source1.CopyTo(result, 0);
            source2.CopyTo(result, source1.Length);
         
            return result;
        }
    }
}
