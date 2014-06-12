using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    public class DynamicMultiMaterialMesh
    {
        public int VertexCount;

        public string Name = "mesh";

        private readonly List<DynamicMultiMaterial> _meshes = new List<DynamicMultiMaterial>();
        private int _subMeshCount;

        public DynamicMultiMaterialMesh()
        {
            _meshes = new List<DynamicMultiMaterial>();
        }

        public DynamicMultiMaterial this[int index]
        {
            get { return _meshes[index]; }
        }

        public int MeshCount
        {
            get { return _meshes.Count; }
        }

        public void Build(bool calcTangents)
        {
            foreach (DynamicMultiMaterial mesh in _meshes)
            {
                mesh.Build(calcTangents);
            }
        }

        public void Clear()
        {
            foreach (DynamicMultiMaterial mesh in _meshes)
                mesh.Clear();
            VertexCount = 0;
            _meshes.Clear();
        }

        public int SubMeshCount
        {
            get { return _subMeshCount; }
            set
            {
                _subMeshCount = value;
                foreach (DynamicMultiMaterial mesh in _meshes)
                    mesh.SubMeshCount = value;
            }
        }

        private int CheckMeshSize(int numberOfNewVerts)
        {
            int meshIndex = _meshes.Count - 1;
            if (meshIndex < 0) //check there is a mesh to begin with
            {
                var newMesh = new DynamicMultiMaterial();
                newMesh.Name = Name + " " + (meshIndex + 2);
                newMesh.SubMeshCount = _subMeshCount;
                _meshes.Add(newMesh);
                VertexCount += numberOfNewVerts;
                return 0;
            }
            int newVertCount = numberOfNewVerts + VertexCount - (meshIndex*65000);
            if (newVertCount >= 64990)
            {
                var newMesh = new DynamicMultiMaterial();
                newMesh.Name = Name + " " + (meshIndex + 2);
                newMesh.SubMeshCount = _subMeshCount;
                _meshes.Add(newMesh);
                meshIndex++;
            }
            VertexCount += numberOfNewVerts;
            return meshIndex;
        }

        public void AddData(Vector3[] verts, Vector2[] uvs, int[] tris, int subMesh)
        {
            int useMeshIndex = CheckMeshSize(verts.Length);
            _meshes[useMeshIndex].AddData(verts, uvs, tris, subMesh);
        }

        /// <summary>
        ///     Adds the plane to the generic dynamic mesh by specifying min and max UV coords.
        /// </summary>
        /// <param name='p0,p1,p2,p3'> 4 Verticies that define the plane </param>
        /// <param name='minUV'> the minimum vertex UV coord. </param>
        /// <param name='maxUV'> the maximum vertex UV coord. </param>
        /// <param name='subMesh'> The sub mesh to attch this plan to - in order of Texture library indicies </param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 minUV, Vector2 maxUV, int subMesh)
        {
            var uv0 = new Vector2(minUV.x, minUV.y);
            var uv1 = new Vector2(maxUV.x, minUV.y);
            var uv2 = new Vector2(minUV.x, maxUV.y);
            var uv3 = new Vector2(maxUV.x, maxUV.y);

            int useMeshIndex = CheckMeshSize(4);
            _meshes[useMeshIndex].AddPlane(p0, p1, p2, p3, uv0, uv1, uv2, uv3, subMesh);
        }
    }
}