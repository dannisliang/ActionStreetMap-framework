using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    /// <summary>
    ///     A class dealing with dynamic mesh generataion. Attempts to keep memory use low.
    ///     Contains a variety of functions to help in the generation of meshes.
    ///     Uses generic lists to contain the mesh Model allowing for the mesh to be of dynamic size
    ///     Creates a mesh that can support multiple materials
    /// </summary>
    public class DynamicMeshGenericMultiMaterial
    {
        public string Name = "";
        public Mesh Mesh;
        public List<Vector3> Vertices;
        public List<Vector2> UV;
        public List<int> Triangles;

        public readonly List<Vector2> MinWorldUVSize = new List<Vector2>();
        public readonly List<Vector2> MaxWorldUVSize = new List<Vector2>();

        private int _subMeshes = 1;
        private readonly Dictionary<int, List<int>> _subTriangles;
        private Vector3[] _tan1;
        private Vector3[] _tan2;
        private Vector4[] _tangents;

        public DynamicMeshGenericMultiMaterial()
        {
            Mesh = new Mesh();
            Vertices = new List<Vector3>();
            UV = new List<Vector2>();
            Triangles = new List<int>();
            _subTriangles = new Dictionary<int, List<int>>();
        }

        public void Build(bool calcTangents)
        {
            //Unity has an inbuilt limit of 65000 verticies. Use DynamicMeshGenericMultiMaterialMesh to handle more than 65000
            if (VertexCount > 65000)
            {
                Debug.LogWarning(Name + " is exceeding 65000 vertices - stop build");
                return;
            }

            //User needs to specify the amount of submeshes this mesh contains
            if (SubMeshCount == 0)
            {
                Debug.LogWarning(Name + " has no submeshes - you need to define them pre build");
                return;
            }

            Mesh.Clear();
            Mesh.name = Name;
            Mesh.vertices = Vertices.ToArray();
            Mesh.uv = UV.ToArray();
            Mesh.uv2 = new Vector2[0];

            Mesh.subMeshCount = _subMeshes;
            List<int> setTris = new List<int>();
            foreach (KeyValuePair<int, List<int>> triData in _subTriangles)
            {
                Mesh.SetTriangles(triData.Value.ToArray(), triData.Key);
                setTris.AddRange(triData.Value);
            }

            Mesh.RecalculateBounds();
            Mesh.RecalculateNormals();

            if (calcTangents)
            {
                SolveTangents();
            }
            else
            {
                Vector4[] emptyTangents = new Vector4[Size];
                Mesh.tangents = emptyTangents;
            }
        }

        /// <summary>
        ///     Clears the mesh Model, ready for a new mesh build
        /// </summary>
        public void Clear()
        {
            Mesh.Clear();
            Vertices.Clear();
            UV.Clear();
            Triangles.Clear();
            _subTriangles.Clear();
            _subMeshes = 0;
        }

        public int VertexCount
        {
            get { return Vertices.Count; }
        }

        public int Size
        {
            get { return Vertices.Count; }
        }


        public int SubMeshCount
        {
            get { return _subMeshes; }
            set
            {
                _subMeshes = value;
                //reset the largest/smallest UZ size monitors
                if (MinWorldUVSize.Count > value)
                    MinWorldUVSize.Clear();
                if (MaxWorldUVSize.Count > value)
                    MaxWorldUVSize.Clear();

                while (MinWorldUVSize.Count <= value)
                {
                    MinWorldUVSize.Add(Vector2.zero);
                    MaxWorldUVSize.Add(Vector2.one);
                }
            }
        }

        /// <summary>
        ///     Generate the Mesh tangents.
        ///     These calculations are heavy and not idea for complex meshes at runtime
        /// </summary>
        public void SolveTangents()
        {
            _tan1 = new Vector3[Size];
            _tan2 = new Vector3[Size];
            _tangents = new Vector4[Size];
            int triangleCount = Triangles.Count/3;

            for (int a = 0; a < triangleCount; a += 3)
            {
                int i1 = Triangles[a + 0];
                int i2 = Triangles[a + 1];
                int i3 = Triangles[a + 2];

                Vector3 v1 = Vertices[i1];
                Vector3 v2 = Vertices[i2];
                Vector3 v3 = Vertices[i3];

                Vector2 w1 = UV[i1];
                Vector2 w2 = UV[i2];
                Vector2 w3 = UV[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f/(s1*t2 - s2*t1);

                Vector3 sdir = new Vector3((t2*x1 - t1*x2)*r, (t2*y1 - t1*y2)*r, (t2*z1 - t1*z2)*r);
                Vector3 tdir = new Vector3((s1*x2 - s2*x1)*r, (s1*y2 - s2*y1)*r, (s1*z2 - s2*z1)*r);

                _tan1[i1] += sdir;
                _tan1[i2] += sdir;
                _tan1[i3] += sdir;

                _tan2[i1] += tdir;
                _tan2[i2] += tdir;
                _tan2[i3] += tdir;
            }


            for (int a = 0; a < Size; ++a)
            {
                Vector3 n = Mesh.normals[a];
                Vector3 t = _tan1[a];

                Vector3 tmp = (t - n*Vector3.Dot(n, t)).normalized;
                _tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

                _tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), _tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }
            Mesh.tangents = _tangents;
        }

        /// <summary>
        ///     Add new mesh Model - all arrays are ordered together
        /// </summary>
        /// <param name="verts">And array of verticies</param>
        /// <param name="uvs">And array of uvs</param>
        /// <param name="tris">And array of triangles</param>
        /// <param name="subMesh">The submesh to add the Model into</param>
        public void AddData(Vector3[] verts, Vector2[] uvs, int[] tris, int subMesh)
        {
            int indiceBase = Vertices.Count;
            Vertices.AddRange(verts);
            UV.AddRange(uvs);
            if (!_subTriangles.ContainsKey(subMesh))
                _subTriangles.Add(subMesh, new List<int>());

            int newTriCount = tris.Length;
            for (int t = 0; t < newTriCount; t++)
            {
                int newTri = (indiceBase + tris[t]);
                Triangles.Add(newTri);
                _subTriangles[subMesh].Add(newTri);
            }

            //calculate the bounds of the UV on the mesh
            Vector2 minimunWorldUVSize = MinWorldUVSize[subMesh];
            Vector2 maximumWorldUVSize = MaxWorldUVSize[subMesh];

            int vertCount = verts.Length;
            for (int i = 0; i < vertCount - 1; i++)
            {
                Vector2 thisuv = uvs[i];
                if (thisuv.x < minimunWorldUVSize.x)
                    minimunWorldUVSize.x = thisuv.x;
                if (thisuv.y < minimunWorldUVSize.y)
                    minimunWorldUVSize.y = thisuv.y;

                if (thisuv.x > maximumWorldUVSize.x)
                    maximumWorldUVSize.x = thisuv.x;
                if (thisuv.y > maximumWorldUVSize.y)
                    maximumWorldUVSize.y = thisuv.y;
            }
            MinWorldUVSize[subMesh] = minimunWorldUVSize;
            MaxWorldUVSize[subMesh] = maximumWorldUVSize;
        }

        /// <summary>
        ///     Adds the plane to the generic dynamic mesh.
        /// </summary>
        /// <param name='p0,p1,p2,p3'>
        ///     4 Verticies that define the plane
        /// </param>
        /// <param name='uv0,uv1,uv2,uv3'>
        ///     the vertex UV coords.
        /// </param>
        /// <param name='subMesh'>
        ///     The sub mesh to attch this plan to - in order of Texture library indicies
        /// </param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv0, Vector2 uv1, Vector2 uv2,
            Vector2 uv3, int subMesh)
        {
            int indiceBase = Vertices.Count;
            Vertices.Add(p0);
            Vertices.Add(p1);
            Vertices.Add(p2);
            Vertices.Add(p3);

            UV.Add(uv0);
            UV.Add(uv1);
            UV.Add(uv2);
            UV.Add(uv3);

            if (!_subTriangles.ContainsKey(subMesh))
                _subTriangles.Add(subMesh, new List<int>());

            _subTriangles[subMesh].Add(indiceBase);
            _subTriangles[subMesh].Add(indiceBase + 2);
            _subTriangles[subMesh].Add(indiceBase + 1);

            _subTriangles[subMesh].Add(indiceBase + 1);
            _subTriangles[subMesh].Add(indiceBase + 2);
            _subTriangles[subMesh].Add(indiceBase + 3);

            Triangles.Add(indiceBase);
            Triangles.Add(indiceBase + 2);
            Triangles.Add(indiceBase + 1);

            Triangles.Add(indiceBase + 1);
            Triangles.Add(indiceBase + 2);
            Triangles.Add(indiceBase + 3);

            Vector2 minWorldUVSize = MinWorldUVSize[subMesh];
            Vector2 maxWorldUVSize = MaxWorldUVSize[subMesh];
            int vertCount = 4;
            Vector2[] uvs = {uv0, uv1, uv2, uv3};
            for (int i = 0; i < vertCount - 1; i++)
            {
                Vector2 thisuv = uvs[i];
                if (thisuv.x < minWorldUVSize.x)
                    minWorldUVSize.x = thisuv.x;
                if (thisuv.y < minWorldUVSize.y)
                    minWorldUVSize.y = thisuv.y;

                if (thisuv.x > maxWorldUVSize.x)
                    maxWorldUVSize.x = thisuv.x;
                if (thisuv.y > maxWorldUVSize.y)
                    maxWorldUVSize.y = thisuv.y;
            }
            MinWorldUVSize[subMesh] = minWorldUVSize;
            MaxWorldUVSize[subMesh] = maxWorldUVSize;
        }
    }
}