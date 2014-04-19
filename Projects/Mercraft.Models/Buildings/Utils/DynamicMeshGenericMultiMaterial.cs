using System.Collections.Generic;
using Mercraft.Models.Buildings.Entities;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Utils
{
    /// <summary>
    /// A class dealing with dynamic mesh generataion. Attempts to keep memory use low. 
    /// Contains a variety of functions to help in the generation of meshes.
    /// Uses generic lists to contain the mesh data allowing for the mesh to be of dynamic size
    /// Creates a mesh that can support multiple materials
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
        private bool _built;
        private bool _hasTangents;

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
            if (vertexCount > 65000)
            {
                Debug.LogWarning(Name + " is exceeding 65000 vertices - stop build");
                _built = false;
                return;
            }

            //User needs to specify the amount of submeshes this mesh contains
            if (subMeshCount == 0)
            {
                Debug.LogWarning(Name + " has no submeshes - you need to define them pre build");
                _built = false;
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
                _hasTangents = false;
                Vector4[] emptyTangents = new Vector4[size];
                Mesh.tangents = emptyTangents;
            }

            optimised = false;
            _built = true;
            lightmapUvsCalculated = false;
        }

        /// <summary>
        /// Clears the mesh data, ready for a new mesh build
        /// </summary>
        public void Clear()
        {
            Mesh.Clear();
            Vertices.Clear();
            UV.Clear();
            Triangles.Clear();
            _subTriangles.Clear();
            _built = false;
            _subMeshes = 0;
        }

        public int vertexCount
        {
            get
            {
                return Vertices.Count;
            }
        }

        public bool built
        {
            get { return _built; }
        }

        public int size
        {
            get { return Vertices.Count; }
        }

        public int triangleCount
        {
            get { return Triangles.Count; }
        }

        public int subMeshCount
        {
            get
            {
                return _subMeshes;
            }
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

        public bool hasTangents { get { return _hasTangents; } }

        public bool lightmapUvsCalculated { get; set; }

        public bool optimised { get; set; }



        /// <summary>
        /// Generate the Mesh tangents.
        /// These calculations are heavy and not idea for complex meshes at runtime
        /// </summary>
        public void SolveTangents()
        {
            _tan1 = new Vector3[size];
            _tan2 = new Vector3[size];
            _tangents = new Vector4[size];
            int triangleCount = Triangles.Count / 3;

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

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                _tan1[i1] += sdir;
                _tan1[i2] += sdir;
                _tan1[i3] += sdir;

                _tan2[i1] += tdir;
                _tan2[i2] += tdir;
                _tan2[i3] += tdir;
            }


            for (int a = 0; a < size; ++a)
            {
                Vector3 n = Mesh.normals[a];
                Vector3 t = _tan1[a];

                Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
                _tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

                _tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), _tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }
            Mesh.tangents = _tangents;
            _hasTangents = true;
        }

        /// <summary>
        /// Add new mesh data - all arrays are ordered together
        /// </summary>
        /// <param name="verts">And array of verticies</param>
        /// <param name="uvs">And array of uvs</param>
        /// <param name="tris">And array of triangles</param>
        /// <param name="subMesh">The submesh to add the data into</param>
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
        /// Add the new triangle to the mesh data
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="subMesh"></param>
        public void AddTri(Vector3 p0, Vector3 p1, Vector3 p2, int subMesh)
        {
            int indiceBase = Vertices.Count;
            Vertices.Add(p0);
            Vertices.Add(p1);
            Vertices.Add(p2);

            UV.Add(new Vector2(0, 0));
            UV.Add(new Vector2(1, 0));
            UV.Add(new Vector2(0, 1));

            if (!_subTriangles.ContainsKey(subMesh))
                _subTriangles.Add(subMesh, new List<int>());

            Triangles.Add(indiceBase);
            Triangles.Add(indiceBase + 2);
            Triangles.Add(indiceBase + 1);

            _subTriangles[subMesh].Add(indiceBase);
            _subTriangles[subMesh].Add(indiceBase + 2);
            _subTriangles[subMesh].Add(indiceBase + 1);
        }

        /// <summary>
        /// Adds the plane to the generic dynamic mesh without specifying UV coords.
        /// </summary>
        /// <param name='p0,p1,p2,p3'>4 Verticies that define the plane</param>
        /// <param name='subMesh'>
        /// The sub mesh to attch this plan to - in order of Texture library indicies
        /// </param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int subMesh)
        {
            AddPlane(p0, p1, p2, p3, Vector2.zero, Vector2.one, subMesh);
        }

        /// <summary>
        /// Adds the plane to the generic dynamic mesh by specifying min and max UV coords.
        /// </summary>
        /// <param name='p0,p1,p2,p3'> 4 Verticies that define the plane</param>
        /// <param name='minUV'> the minimum vertex UV coord. </param>
        /// <param name='maxUV'> the maximum vertex UV coord. </param>
        /// <param name='subMesh'>
        /// The sub mesh to attch this plan to - in order of Texture library indicies
        /// </param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 minUV, Vector2 maxUV, int subMesh)
        {
            Vector2 uv0 = new Vector2(minUV.x, minUV.y);
            Vector2 uv1 = new Vector2(maxUV.x, minUV.y);
            Vector2 uv2 = new Vector2(minUV.x, maxUV.y);
            Vector2 uv3 = new Vector2(maxUV.x, maxUV.y);

            AddPlane(p0, p1, p2, p3, uv0, uv1, uv2, uv3, subMesh);
        }

        /// <summary>
        /// Adds the plane to the generic dynamic mesh.
        /// </summary>
        /// <param name='p0,p1,p2,p3'>
        /// 4 Verticies that define the plane
        /// </param>
        /// <param name='uv0,uv1,uv2,uv3'>
        /// the vertex UV coords.
        /// </param>
        /// <param name='subMesh'>
        /// The sub mesh to attch this plan to - in order of Texture library indicies
        /// </param>
        public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, int subMesh)
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
            Vector2[] uvs = { uv0, uv1, uv2, uv3 };
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

        /// <summary>
        /// Checks the Max UV values used in this model for each texture.
        /// </summary>
        /// <param name='data'>
        /// BuildR Data.
        /// </param>
        public void CheckMaxTextureUVs(Data data)
        {
            Vector2[] subMeshUVOffsets = new Vector2[subMeshCount];
            int[] subMeshIDs = new List<int>(_subTriangles.Keys).ToArray();
            int numberOfSubmeshIDs = subMeshIDs.Length;
            for (int sm = 0; sm < numberOfSubmeshIDs; sm++)
            {
                int subMeshID = subMeshIDs[sm];
                if (_subTriangles.ContainsKey(subMeshID))
                {
                    int[] submeshIndices = _subTriangles[subMeshID].ToArray();
                    subMeshUVOffsets[sm] = Vector2.zero;
                    foreach (int index in submeshIndices)
                    {
                        if (UV[index].x < subMeshUVOffsets[sm].x)
                            subMeshUVOffsets[sm].x = UV[index].x;
                        if (UV[index].y < subMeshUVOffsets[sm].y)
                            subMeshUVOffsets[sm].y = UV[index].y;
                    }

                    List<int> UVsOffset = new List<int>();
                    foreach (int uvindex in _subTriangles[subMeshID])
                    {
                        if (!UVsOffset.Contains(uvindex))
                        {
                            UV[uvindex] += -subMeshUVOffsets[sm];//offset the UV to ensure it isn't negative
                            UVsOffset.Add(uvindex);
                        }
                        data.Textures[subMeshID].CheckMaxUV(UV[uvindex]);
                    }
                }
                else
                {
                    Debug.Log("Mesh does not contain key for texture " + data.Textures[subMeshID].Name);
                }
            }
        }

        /// <summary>
        ///  Atlas the entire mesh using newTextureCoords and textures.
        /// </summary>
        /// <param name="newTextureCoords"></param>
        /// <param name="textures"></param>
        public void Atlas(Rect[] newTextureCoords, Texture[] textures)
        {
            List<int> keys = new List<int>(_subTriangles.Keys);
            Atlas(keys.ToArray(), newTextureCoords, textures);
        }

        /// <summary>
        /// Atlas the specified modifySubmeshes using newTextureCoords and textures.
        /// </summary>
        /// <param name='modifySubmeshes'>
        /// Submeshes indicies to atlas.
        /// </param>
        /// <param name='newTextureCoords'>
        /// New texture coords generated from Pack Textures.
        /// </param>
        /// <param name='textures'>
        /// BuildR Textures library reference.
        /// </param>
        public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords, Texture[] textures)
        {
            if (modifySubmeshes.Length == 0)
            {
                Debug.Log("No submeshes to atlas!");
                return;
            }
            List<int> atlasedSubmesh = new List<int>();
            int numberOfSubmeshesToModify = modifySubmeshes.Length;
            for (int s = 0; s < numberOfSubmeshesToModify; s++)
            {
                int submeshIndex = modifySubmeshes[s];
                Rect textureRect = newTextureCoords[s];
                if (!_subTriangles.ContainsKey(submeshIndex))
                    continue;
                int[] submeshIndices = _subTriangles[submeshIndex].ToArray();
                _subTriangles.Remove(submeshIndex);
                atlasedSubmesh.AddRange(submeshIndices);

                Texture bTexture = textures[submeshIndex];
                List<int> ModifiedUVs = new List<int>();
                foreach (int index in submeshIndices)
                {
                    if (ModifiedUVs.Contains(index))
                        continue;//don't move the UV more than once
                    Vector2 uvCoords = UV[index];
                    float xUV = uvCoords.x / bTexture.MaxUVTile.x;
                    float yUV = uvCoords.y / bTexture.MaxUVTile.y;
                    if (xUV > 1)
                    {
                        bTexture.MaxUVTile.x = uvCoords.x;
                        xUV = 1.0f;
                    }
                    if (yUV > 1)
                    {
                        bTexture.MaxUVTile.y = uvCoords.y;
                        yUV = 1;
                    }

                    uvCoords.x = Mathf.Lerp(textureRect.xMin, textureRect.xMax, xUV);
                    uvCoords.y = Mathf.Lerp(textureRect.yMin, textureRect.yMax, yUV);

                    if (float.IsNaN(uvCoords.x))
                    {
                        uvCoords.x = 1;
                    }
                    if (float.IsNaN(uvCoords.y))
                    {
                        uvCoords.y = 1;
                    }
                    UV[index] = uvCoords;
                    ModifiedUVs.Add(index);//keep a record of moved UVs
                }
            }
            subMeshCount = subMeshCount - modifySubmeshes.Length + 1;
            _subTriangles.Add(modifySubmeshes[0], atlasedSubmesh);
        }

        /// <summary>
        /// Atlas the entire mesh, specifying specific submeshes that have been atlased
        /// </summary>
        /// <param name="modifySubmeshes">Specified submeshes for the atlased coords</param>
        /// <param name="newTextureCoords">New texture coords generated from Pack Textures.</param>
        public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords)
        {
            if (modifySubmeshes.Length == 0)
            {
                Debug.Log("No submeshes to atlas!");
                return;
            }
            List<int> atlasedSubmesh = new List<int>();
            List<int> modifySubmeshList = new List<int>(modifySubmeshes);
            for (int s = 0; s < subMeshCount; s++)
            {
                if (!_subTriangles.ContainsKey(s))
                    continue;

                int[] submeshIndices = _subTriangles[s].ToArray();
                _subTriangles.Remove(s);
                atlasedSubmesh.AddRange(submeshIndices);

                if (modifySubmeshList.Contains(s))
                {
                    Rect textureRect = newTextureCoords[s];
                    List<int> ModifiedUVs = new List<int>();
                    foreach (int index in submeshIndices)
                    {
                        if (ModifiedUVs.Contains(index))
                            continue;//don't move the UV more than once
                        Vector2 uvCoords = UV[index];
                        uvCoords.x = Mathf.Lerp(textureRect.xMin, textureRect.xMax, uvCoords.x);
                        uvCoords.y = Mathf.Lerp(textureRect.yMin, textureRect.yMax, uvCoords.y);
                        UV[index] = uvCoords;
                        ModifiedUVs.Add(index);//keep a record of moved UVs
                    }
                }
                else
                {
                    List<int> ModifiedUVs = new List<int>();
                    foreach (int index in submeshIndices)
                    {
                        if (ModifiedUVs.Contains(index))
                            continue;//don't move the UV more than once
                        UV[index] = Vector2.zero;
                        ModifiedUVs.Add(index);//keep a record of moved UVs
                    }
                }
            }
            //subMeshCount = subMeshCount - modifySubmeshes.Length + 1;
            subMeshCount = 1;
            _subTriangles.Add(modifySubmeshes[0], atlasedSubmesh);
        }

        /// <summary>
        /// Collapse all the submeshes into a single submesh
        /// </summary>
        public void CollapseSubmeshes()
        {
            List<int> atlasedSubmesh = new List<int>();
            int numberOfSubmeshesToModify = subMeshCount;
            for (int s = 0; s < numberOfSubmeshesToModify; s++)
            {
                if (_subTriangles.ContainsKey(s))
                {
                    int[] submeshIndices = _subTriangles[s].ToArray();
                    atlasedSubmesh.AddRange(submeshIndices);
                }
            }
            subMeshCount = 1;
            _subTriangles.Clear();
            _subTriangles.Add(0, atlasedSubmesh);
        }

        public void RemoveRedundantVerticies()
        {
            List<Vector3> vertList = new List<Vector3>();
            int vertIndex = 0;
            int numberOfVerts = vertexCount;

            for (int i = 0; i < numberOfVerts; i++)
            {
                Vector3 vert = Vertices[i];
                if (!vertList.Contains(vert))
                {
                    //no redundancy - add
                    vertList.Add(vert);
                }
                else
                {
                    //possible redundancy
                    int firstIndex = Vertices.IndexOf(vert);
                    int secondIndex = vertIndex;

                    //check to see if these verticies are connected
                    if (UV[firstIndex] == UV[secondIndex])
                    {
                        //verticies are connected - merge them

                        int triIndex = 0;
                        while ((triIndex = Triangles.IndexOf(secondIndex)) != -1)
                            Triangles[triIndex] = firstIndex;

                        Vertices.RemoveAt(secondIndex);
                        UV.RemoveAt(secondIndex);

                        numberOfVerts--;
                        i--;
                    }
                    else
                    {
                        vertList.Add(vert);
                    }
                }

                vertIndex++;
            }
        }
    }
}