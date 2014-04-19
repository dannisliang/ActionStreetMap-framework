using System.Collections.Generic;
using Mercraft.Models.Buildings.Entities;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Utils
{
    public class DynamicMeshGenericMultiMaterialMesh
    {
        public int VertexCount;

        public string Name = "mesh";

        private readonly List<DynamicMeshGenericMultiMaterial> _meshes = new List<DynamicMeshGenericMultiMaterial>();
        private int _subMeshCount;


        public DynamicMeshGenericMultiMaterialMesh()
        {
            _meshes = new List<DynamicMeshGenericMultiMaterial>();
        }

        public DynamicMeshGenericMultiMaterial this[int index]
        {
            get { return _meshes[index]; }
        }

        public int MeshCount
        {
            get { return _meshes.Count; }
        }

        public Vector3[] Vertices
        {
            get
            {
                var returnVerts = new List<Vector3>();
                foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                    returnVerts.AddRange(mesh.Vertices);
                return returnVerts.ToArray();
            }
        }

        public Vector2[] UV
        {
            get
            {
                var returnUVs = new List<Vector2>();
                foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                    returnUVs.AddRange(mesh.UV);
                return returnUVs.ToArray();
            }
        }

        public int[] Triangles
        {
            get
            {
                var returnTris = new List<int>();
                foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                    returnTris.AddRange(mesh.Triangles);
                return returnTris.ToArray();
            }
        }

        public Vector2 MinWorldUvSize(int submesh)
        {
            Vector2 mainMinUV = _meshes[0].MinWorldUVSize[0];
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            {
                for (int i = 0; i < _subMeshCount; i++)
                {
                    Vector2 meshMinUV = mesh.MinWorldUVSize[i];
                    if (meshMinUV.x < mainMinUV.x) mainMinUV.x = meshMinUV.x;
                    if (meshMinUV.y < mainMinUV.y) mainMinUV.y = meshMinUV.y;
                }
            }
            return mainMinUV;
        }

        public Vector2 MaxWorldUvSize(int submesh)
        {
            Vector2 mainMinUV = _meshes[0].MinWorldUVSize[0];
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            {
                for (int i = 0; i < _subMeshCount; i++)
                {
                    Vector2 meshMinUV = mesh.MinWorldUVSize[i];
                    if (meshMinUV.x > mainMinUV.x) mainMinUV.x = meshMinUV.x;
                    if (meshMinUV.y > mainMinUV.y) mainMinUV.y = meshMinUV.y;
                }
            }
            return mainMinUV;
        }

        public void Build()
        {
            Build(false);
        }

        public void Build(bool calcTangents)
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            {
                mesh.Build(calcTangents);
            }
        }

        public void Clear()
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.Clear();
            VertexCount = 0;
            _meshes.Clear();
        }



      

       

        public int SubMeshCount
        {
            get
            {
                return _subMeshCount;
            }
            set
            {
                _subMeshCount = value;
                foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                    mesh.subMeshCount = value;
            }
        }     

       
        private int CheckMeshSize(int numberOfNewVerts)
        {
            int meshIndex = _meshes.Count - 1;
            if (meshIndex < 0) //check there is a mesh to begin with
            {
                var newMesh = new DynamicMeshGenericMultiMaterial();
                newMesh.Name = Name + " " + (meshIndex + 2);
                newMesh.subMeshCount = _subMeshCount;
                _meshes.Add(newMesh);
                VertexCount += numberOfNewVerts;
                return 0;
            }
            int newVertCount = numberOfNewVerts + VertexCount - (meshIndex*65000);
            if (newVertCount >= 64990)
            {
                var newMesh = new DynamicMeshGenericMultiMaterial();
                newMesh.Name = Name + " " + (meshIndex + 2);
                newMesh.subMeshCount = _subMeshCount;
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

        public void AddTri(Vector3 p0, Vector3 p1, Vector3 p2, int subMesh)
        {
            int useMeshIndex = CheckMeshSize(3);
            _meshes[useMeshIndex].AddTri(p0, p1, p2, subMesh);
        }

        /// <summary>
        /// Adds the plane to the generic dynamic mesh without specifying UV coords.
        /// </summary>
        /// <param name='p0,p1,p2,p3'> 4 Verticies that define the plane </param>
        /// <param name='subMesh'> The sub mesh to attch this plan to - in order of Texture library indicies </param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int subMesh)
        {
            int useMeshIndex = CheckMeshSize(4);
            _meshes[useMeshIndex].AddPlane(p0, p1, p2, p3, Vector2.zero, Vector2.one, subMesh);
        }

        /// <summary>
        /// Adds the plane to the generic dynamic mesh by specifying min and max UV coords.
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

        /// <summary>
        /// Adds the plane to the generic dynamic mesh.
        /// </summary>
        /// <param name='p0,p1,p2,p3'> 4 Verticies that define the plane </param>
        /// <param name='uv0,uv1,uv2,uv3'> the vertex UV coords. </param>
        /// <param name='subMesh'> The sub mesh to attch this plan to - in order of Texture library indicies</param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv0, Vector2 uv1, Vector2 uv2,
            Vector2 uv3, int subMesh)
        {
            int useMeshIndex = CheckMeshSize(4);
            _meshes[useMeshIndex].AddPlane(p0, p1, p2, p3, uv0, uv1, uv2, uv3, subMesh);
        }

        /// <summary>
        /// Checks the Max UV values used in this model for each texture.
        /// </summary>
        public void CheckMaxTextureUVs(Data data)
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.CheckMaxTextureUVs(data);
        }

        public void Atlas(Rect[] newTextureCoords, Texture[] textures)
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.Atlas(newTextureCoords, textures);
        }

        public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords)
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.Atlas(modifySubmeshes, newTextureCoords);
        }

        /// <summary>
        /// Atlas the specified modifySubmeshes using newTextureCoords and textures.
        /// </summary>
        /// <param name='modifySubmeshes'> Submeshes indicies to atlas. </param>
        /// <param name='newTextureCoords'> New texture coords generated from Pack Textures. </param>
        /// <param name='textures'> Textures library reference. </param>
        public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords, Texture[] textures)
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.Atlas(modifySubmeshes, newTextureCoords, textures);
        }

        public void CollapseSubmeshes()
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.CollapseSubmeshes();
        }
    }
}