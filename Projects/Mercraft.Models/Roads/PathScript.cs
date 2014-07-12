/* Modified version of free Road Tool script
 * TODO Should be optimized
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class PathScript
    {
        public bool addNodeMode;
        public bool isRoad = true;
        public bool isFinalized;

        public PathNodeObject[] nodeObjects;
        // public Vector3[] nodeObjectVerts; // keeps vertice positions for handles

        public MeshCollider pathCollider;

        // GUI variables
        public int pathWidth;
        public int pathTexture;
        public bool pathUniform = true;
        public bool pathFlat = true;
        public bool showHandles;
        public float pathWear = 1f;
        public int pathSmooth = 5;

        public float heightY = 0.1f;

        // name of the terrain that created the path
        public GameObject parentTerrain;

        public GameObject terrainObj;
        public UnityEngine.Terrain terComponent;
        public TerrainData terrainData;

        private GameObject _pathObject;

        public void NewPath(GameObject pathObject, GameObject terrainObject)
        {
            _pathObject = pathObject;
            nodeObjects = new PathNodeObject[0];
            pathCollider = (MeshCollider)_pathObject.AddComponent(typeof(MeshCollider));

            parentTerrain = terrainObject;

            terComponent = (UnityEngine.Terrain)terrainObject.GetComponent(typeof(UnityEngine.Terrain));

            terrainData = terComponent.terrainData;
        }

        public void CreatePathNode(Vector3 nodeCell)
        {
            Vector3 pathPosition = new Vector3((nodeCell.x / terrainData.heightmapResolution) * terrainData.size.x,
                nodeCell.y * terrainData.size.y, (nodeCell.z / terrainData.heightmapResolution) * terrainData.size.z);

            AddNode(pathPosition, pathWidth);

            CreatePath(pathSmooth, true, false);
        }

        public void AddNode(Vector3 position, float width)
        {
            PathNodeObject newPathNodeObject = new PathNodeObject();
            int nNodes;

            if (nodeObjects == null)
            {
                nodeObjects = new PathNodeObject[0];
                nNodes = 1;
                newPathNodeObject.Position = position;
            }

            else
            {
                nNodes = nodeObjects.Length + 1;
                newPathNodeObject.Position = position;
            }

            PathNodeObject[] newNodeObjects = new PathNodeObject[nNodes];
            newPathNodeObject.Width = width;

            int n = newNodeObjects.Length;

            for (int i = 0; i < n; i++)
            {
                if (i != n - 1)
                {
                    newNodeObjects[i] = nodeObjects[i];
                }

                else
                {
                    newNodeObjects[i] = newPathNodeObject;
                }
            }

            nodeObjects = newNodeObjects;
        }

        public void CreatePath(int smoothingLevel, bool flatten, bool road)
        {
            MeshFilter meshFilter = (MeshFilter)_pathObject.GetComponent(typeof(MeshFilter));

            if (meshFilter == null)
                return;

            Mesh newMesh = meshFilter.sharedMesh;
            //terrainHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);


            if (newMesh == null)
            {
                newMesh = new Mesh();
                newMesh.name = "Generated Path Mesh";
                meshFilter.sharedMesh = newMesh;
            }

            else
                newMesh.Clear();


            if (nodeObjects == null || nodeObjects.Length < 2)
            {
                return;
            }

            int n = nodeObjects.Length;

            int verticesPerNode = 2 * (smoothingLevel + 1) * 2;
            int trianglesPerNode = 6 * (smoothingLevel + 1);
            Vector2[] uvs = new Vector2[(verticesPerNode * (n - 1))];
            Vector3[] newVertices = new Vector3[(verticesPerNode * (n - 1))];
            int[] newTriangles = new int[(trianglesPerNode * (n - 1))];
            int nextVertex = 0;
            int nextTriangle = 0;
            int nextUV = 0;

            // variables for splines and perpendicular extruded points
            float[] cubicX = new float[n];
            float[] cubicZ = new float[n];
            Vector3 handle1Tween = new Vector3();
            Vector3[] g1 = new Vector3[smoothingLevel + 1];
            Vector3[] g2 = new Vector3[smoothingLevel + 1];
            Vector3[] g3 = new Vector3[smoothingLevel + 1];
            Vector3 oldG2 = new Vector3();
            Vector3 extrudedPointL = new Vector3();
            Vector3 extrudedPointR = new Vector3();

            for (int i = 0; i < n; i++)
            {
                cubicX[i] = nodeObjects[i].Position.x;
                cubicZ[i] = nodeObjects[i].Position.z;
            }

            for (int i = 0; i < n; i++)
            {
                g1 = new Vector3[smoothingLevel + 1];
                g2 = new Vector3[smoothingLevel + 1];
                g3 = new Vector3[smoothingLevel + 1];

                extrudedPointL = new Vector3();
                extrudedPointR = new Vector3();

                if (i == 0)
                {
                    newVertices[nextVertex] = nodeObjects[0].Position;
                    nextVertex++;
                    uvs[0] = new Vector2(0f, 1f);
                    nextUV++;
                    newVertices[nextVertex] = nodeObjects[0].Position;
                    nextVertex++;
                    uvs[1] = new Vector2(1f, 1f);
                    nextUV++;

                    continue;
                }

                float _widthAtNode = pathWidth;

                // Interpolate points along the path using splines for direction and bezier curves for heights
                for (int j = 0; j < smoothingLevel + 1; j++)
                {
                    // clone the vertex for uvs
                    if (i == 1)
                    {
                        if (j != 0)
                        {
                            newVertices[nextVertex] = newVertices[nextVertex - 2];
                            nextVertex++;

                            newVertices[nextVertex] = newVertices[nextVertex - 2];
                            nextVertex++;

                            uvs[nextUV] = new Vector2(0f, 1f);
                            nextUV++;
                            uvs[nextUV] = new Vector2(1f, 1f);
                            nextUV++;
                        }

                        else
                            oldG2 = nodeObjects[0].Position;
                    }

                    else
                    {
                        newVertices[nextVertex] = newVertices[nextVertex - 2];
                        nextVertex++;

                        newVertices[nextVertex] = newVertices[nextVertex - 2];
                        nextVertex++;

                        uvs[nextUV] = new Vector2(0f, 1f);
                        nextUV++;
                        uvs[nextUV] = new Vector2(1f, 1f);
                        nextUV++;
                    }

                    float u = j / (smoothingLevel + 1f);

                    Cubic[] arrCubicX = CalcNaturalCubic(n - 1, cubicX);
                    Cubic[] arrCubicz = CalcNaturalCubic(n - 1, cubicZ);

                    Vector3 tweenPoint = new Vector3(arrCubicX[i - 1].Eval(u), 0f, arrCubicz[i - 1].Eval(u));

                    // update tweened points
                    g2[j] = tweenPoint;
                    g1[j] = oldG2;
                    g3[j] = g2[j] - g1[j];
                    oldG2 = g2[j];

                    // Create perpendicular points for vertices
                    extrudedPointL = new Vector3(-g3[j].z, 0, g3[j].x);
                    extrudedPointR = new Vector3(g3[j].z, 0, -g3[j].x);
                    extrudedPointL.Normalize();
                    extrudedPointR.Normalize();
                    extrudedPointL *= _widthAtNode;
                    extrudedPointR *= _widthAtNode;

                    // Height at the terrain
                    tweenPoint.y = 0;
                    //terrainHeights[GetIndexFromXPoint(tweenPoint), GetIndexFromZPoint(tweenPoint)] * terrainData.size.y + parentTerrain.transform.position.y;

                    // create vertices at the perpendicular points
                    newVertices[nextVertex] = tweenPoint + extrudedPointR;

                    newVertices[nextVertex].y = heightY;
                    // (float)terrainHeights[GetIndexFromZPoint(newVertices[nextVertex]), GetIndexFromXPoint(newVertices[nextVertex])] * terrainData.size.y + parentTerrain.transform.position.y;
                    nextVertex++;

                    newVertices[nextVertex] = tweenPoint + extrudedPointL;

                    newVertices[nextVertex].y = heightY;
                    // (float)terrainHeights[GetIndexFromZPoint(newVertices[nextVertex]), GetIndexFromXPoint(newVertices[nextVertex])] * terrainData.size.y + parentTerrain.transform.position.y;
                    nextVertex++;

                    uvs[nextUV] = new Vector2(0f, 0f);
                    nextUV++;
                    uvs[nextUV] = new Vector2(1f, 0f);
                    nextUV++;

                    // used later to update the handles
                    if (i == 1)
                        if (j == 0)
                            handle1Tween = tweenPoint;

                    // flatten mesh
                    if (flatten && !road)
                    {
                        if (newVertices[nextVertex - 1].y < (newVertices[nextVertex - 2].y - 0.0f))
                        {
                            extrudedPointL *= 1.5f;
                            extrudedPointR *= 1.2f;
                            newVertices[nextVertex - 1] = tweenPoint + extrudedPointL;
                            newVertices[nextVertex - 2] = tweenPoint + extrudedPointR;

                            newVertices[nextVertex - 1].y = newVertices[nextVertex - 2].y;
                        }

                        else if (newVertices[nextVertex - 1].y > (newVertices[nextVertex - 2].y - 0.0f))
                        {
                            extrudedPointR *= 1.5f;
                            extrudedPointL *= 1.2f;
                            newVertices[nextVertex - 2] = tweenPoint + extrudedPointR;
                            newVertices[nextVertex - 1] = tweenPoint + extrudedPointL;

                            newVertices[nextVertex - 2].y = newVertices[nextVertex - 1].y;
                        }
                    }

                    // Create triangles...
                    newTriangles[nextTriangle] = (verticesPerNode * (i - 1)) + (4 * j); // 0
                    nextTriangle++;
                    newTriangles[nextTriangle] = (verticesPerNode * (i - 1)) + (4 * j) + 1; // 1
                    nextTriangle++;
                    newTriangles[nextTriangle] = (verticesPerNode * (i - 1)) + (4 * j) + 2; // 2
                    nextTriangle++;
                    newTriangles[nextTriangle] = (verticesPerNode * (i - 1)) + (4 * j) + 1; // 1
                    nextTriangle++;
                    newTriangles[nextTriangle] = (verticesPerNode * (i - 1)) + (4 * j) + 3; // 3
                    nextTriangle++;
                    newTriangles[nextTriangle] = (verticesPerNode * (i - 1)) + (4 * j) + 2; // 2
                    nextTriangle++;
                }
            }

            // update handles
            g2[0] = handle1Tween;
            g1[0] = nodeObjects[0].Position;
            g3[0] = g2[0] - g1[0];

            extrudedPointL = new Vector3(-g3[0].z, 0, g3[0].x);
            extrudedPointR = new Vector3(g3[0].z, 0, -g3[0].x);

            extrudedPointL.Normalize();
            extrudedPointR.Normalize();
            extrudedPointL *= nodeObjects[0].Width;
            extrudedPointR *= nodeObjects[0].Width;

            newVertices[0] = nodeObjects[0].Position + extrudedPointR;
            newVertices[0].y = heightY;
            newVertices[1] = nodeObjects[0].Position + extrudedPointL;
            newVertices[1].y = heightY;

            if (road)
            {
                for (int i = 0; i < newVertices.Length; i++)
                {
                    newVertices[i].y = heightY;
                }
            }

            newMesh.vertices = newVertices;
            newMesh.triangles = newTriangles;

            newMesh.uv = uvs;

            Vector3[] myNormals = new Vector3[newMesh.vertexCount];
            for (int p = 0; p < newMesh.vertexCount; p++)
            {
                myNormals[p] = Vector3.up;
            }

            newMesh.normals = myNormals;

            TangentSolver(newMesh);

            newMesh.RecalculateNormals();
            pathCollider.sharedMesh = meshFilter.sharedMesh;
            pathCollider.smoothSphereCollisions = true;


            _pathObject.renderer.enabled = true;

            _pathObject.transform.localScale = new Vector3(1, 1, 1);
        }

        public bool FinalizePath(float[, ,] alphamap)
        {
            _pathObject.transform.localScale = new Vector3(1, 1, 1);
            //_pathObject.transform.Translate(0f, -150f, 0f);
            /*pathWidth += 6;
            CreatePath(30, true, false);
            pathWidth -= 7;
            CreatePath(30, true, false);
            pathWidth += 15;
            CreatePath(30, true, false);
            pathWidth -= 12;
            CreatePath(100, true, false);*/
            //_pathObject.transform.Translate(0f, 150f, 0f);
            CreatePath(pathSmooth, true, true);
            isFinalized = true;
            return true;
        }

        public void TangentSolver(Mesh theMesh)
        {
            int vertexCount = theMesh.vertexCount;
            Vector3[] vertices = theMesh.vertices;
            Vector3[] normals = theMesh.normals;
            Vector2[] texcoords = theMesh.uv;
            int[] triangles = theMesh.triangles;
            int triangleCount = triangles.Length / 3;
            Vector4[] tangents = new Vector4[vertexCount];
            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            int tri = 0;

            int i1, i2, i3;
            Vector3 v1, v2, v3, w1, w2, w3, sdir, tdir;
            float x1, x2, y1, y2, z1, z2, s1, s2, t1, t2, r;
            for (int i = 0; i < (triangleCount); i++)
            {
                i1 = triangles[tri];
                i2 = triangles[tri + 1];
                i3 = triangles[tri + 2];

                v1 = vertices[i1];
                v2 = vertices[i2];
                v3 = vertices[i3];

                w1 = texcoords[i1];
                w2 = texcoords[i2];
                w3 = texcoords[i3];

                x1 = v2.x - v1.x;
                x2 = v3.x - v1.x;
                y1 = v2.y - v1.y;
                y2 = v3.y - v1.y;
                z1 = v2.z - v1.z;
                z2 = v3.z - v1.z;

                s1 = w2.x - w1.x;
                s2 = w3.x - w1.x;
                t1 = w2.y - w1.y;
                t2 = w3.y - w1.y;

                r = 1.0f / (s1 * t2 - s2 * t1);
                sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;

                tri += 3;
            }

            for (int i = 0; i < (vertexCount); i++)
            {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];

                // Gram-Schmidt orthogonalize
                Vector3.OrthoNormalize(ref n, ref t);

                tangents[i].x = t.x;
                tangents[i].y = t.y;
                tangents[i].z = t.z;

                // Calculate handedness
                tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
            }

            theMesh.tangents = tangents;
        }

        public Cubic[] CalcNaturalCubic(int n, float[] x)
        {
            float[] gamma = new float[n + 1];
            float[] delta = new float[n + 1];
            float[] D = new float[n + 1];
            int i;

            gamma[0] = 1.0f / 2.0f;

            for (i = 1; i < n; i++)
            {
                gamma[i] = 1 / (4 - gamma[i - 1]);
            }

            gamma[n] = 1 / (2 - gamma[n - 1]);

            delta[0] = 3 * (x[1] - x[0]) * gamma[0];

            for (i = 1; i < n; i++)
            {
                delta[i] = (3 * (x[i + 1] - x[i - 1]) - delta[i - 1]) * gamma[i];
            }

            delta[n] = (3 * (x[n] - x[n - 1]) - delta[n - 1]) * gamma[n];

            D[n] = delta[n];

            for (i = n - 1; i >= 0; i--)
            {
                D[i] = delta[i] - gamma[i] * D[i + 1];
            }

            Cubic[] C = new Cubic[n + 1];
            for (i = 0; i < n; i++)
            {
                C[i] = new Cubic(x[i], D[i], 3 * (x[i + 1] - x[i]) - 2 * D[i] - D[i + 1],
                    2 * (x[i] - x[i + 1]) + D[i] + D[i + 1]);
            }

            return C;
        }
    }
}
