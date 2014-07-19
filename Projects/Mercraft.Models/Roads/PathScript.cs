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
        private PathNodeObject[] _nodeObjects;
        private MeshCollider _pathCollider;

        public int PathWidth;
        public int PathSmooth = 10;
        public float HeightY = 0.1f;


        private GameObject _pathObject;

        public void NewPath(GameObject pathObject, GameObject terrainObject)
        {
            _pathObject = pathObject;
            _pathCollider = (MeshCollider)_pathObject.AddComponent(typeof(MeshCollider));
            _nodeObjects = new PathNodeObject[0];
        }

        public void AddNode(Vector3 position, float width)
        {
            var newPathNodeObject = new PathNodeObject();
            int nNodes;

            if (_nodeObjects == null)
            {
                _nodeObjects = new PathNodeObject[0];
                nNodes = 1;
                newPathNodeObject.Position = position;
            }
            else
            {
                nNodes = _nodeObjects.Length + 1;
                newPathNodeObject.Position = position;
            }

            var newNodeObjects = new PathNodeObject[nNodes];
            newPathNodeObject.Width = width;

            int n = newNodeObjects.Length;

            for (int i = 0; i < n; i++)
            {
                if (i != n - 1)
                {
                    newNodeObjects[i] = _nodeObjects[i];
                }

                else
                {
                    newNodeObjects[i] = newPathNodeObject;
                }
            }

            _nodeObjects = newNodeObjects;
        }

        private void CreatePath(int smoothingLevel)
        {
            var meshFilter = (MeshFilter)_pathObject.GetComponent(typeof(MeshFilter));

            if (meshFilter == null)
                return;

            Mesh newMesh = meshFilter.sharedMesh;

            if (newMesh == null)
            {
                newMesh = new Mesh();
                newMesh.name = "Generated Path Mesh";
                meshFilter.sharedMesh = newMesh;
            }

            else
                newMesh.Clear();

            int n = _nodeObjects.Length;

            int verticesPerNode = 2 * (smoothingLevel + 1) * 2;
            int trianglesPerNode = 6 * (smoothingLevel + 1);
            var uvs = new Vector2[(verticesPerNode * (n - 1))];
            var newVertices = new Vector3[(verticesPerNode * (n - 1))];
            var newTriangles = new int[(trianglesPerNode * (n - 1))];
            int nextVertex = 0;
            int nextTriangle = 0;
            int nextUV = 0;

            // variables for splines and perpendicular extruded points
            var cubicX = new float[n];
            var cubicZ = new float[n];

            var lastPointDirection = (_nodeObjects[n - 1].Position - _nodeObjects[n - 2].Position);
            _nodeObjects[n - 1].Position =  new Vector3(
                  _nodeObjects[n - 1].Position.x + lastPointDirection.x / 8,
                  _nodeObjects[n - 1].Position.y,
                  _nodeObjects[n - 1].Position.z + lastPointDirection.z / 8);
            var oldG2 = new Vector3();

            for (int i = 0; i < n; i++)
            {
                cubicX[i] = _nodeObjects[i].Position.x;
                cubicZ[i] = _nodeObjects[i].Position.z;
            }

            for (int i = 0; i < n; i++)
            {
                var g1 = new Vector3[smoothingLevel + 1];
                var g2 = new Vector3[smoothingLevel + 1];
                var g3 = new Vector3[smoothingLevel + 1];

                if (i == 0)
                {
                    newVertices[nextVertex++] = _nodeObjects[0].Position;
                    uvs[0] = new Vector2(0f, 1f);
                    nextUV++;

                    newVertices[nextVertex++] = _nodeObjects[0].Position;
                    uvs[1] = new Vector2(1f, 1f);
                    nextUV++;

                    continue;
                }

                float widthAtNode = _nodeObjects[i].Width;

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

                            uvs[nextUV++] = new Vector2(0f, 1f);
                            uvs[nextUV++] = new Vector2(1f, 1f);
                        }

                        else
                        {
                            var direction = (_nodeObjects[0].Position - _nodeObjects[1].Position);
                            oldG2 = new Vector3(
                                _nodeObjects[0].Position.x + direction.x,
                                _nodeObjects[0].Position.y + direction.y,
                                _nodeObjects[0].Position.z + direction.z);
                        }
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

                    var tweenPoint = new Vector3(arrCubicX[i - 1].Eval(u), 0f, arrCubicz[i - 1].Eval(u));

                    // update tweened points
                    g2[j] = tweenPoint;
                    g1[j] = oldG2;
                    g3[j] = g2[j] - g1[j];
                    oldG2 = g2[j];

                    // Create perpendicular points for vertices
                    var extrudedPointL = new Vector3(-g3[j].z, 0, g3[j].x);
                    var extrudedPointR = new Vector3(g3[j].z, 0, -g3[j].x);

                    extrudedPointL.Normalize();
                    extrudedPointR.Normalize();
                    extrudedPointL *= widthAtNode;
                    extrudedPointR *= widthAtNode;

                    // Height at the terrain
                    tweenPoint.y = HeightY;

                    newVertices[nextVertex] = tweenPoint + extrudedPointR;
                    newVertices[nextVertex].y = HeightY;
                    nextVertex++;

                    newVertices[nextVertex] = tweenPoint + extrudedPointL;
                    newVertices[nextVertex].y = HeightY;
                    nextVertex++;

                    uvs[nextUV++] = new Vector2(0f, 0f);
                    uvs[nextUV++] = new Vector2(1f, 0f);

                    // Create triangles...
                    newTriangles[nextTriangle++] = (verticesPerNode * (i - 1)) + (4 * j); // 0
                    newTriangles[nextTriangle++] = (verticesPerNode * (i - 1)) + (4 * j) + 1; // 1
                    newTriangles[nextTriangle++] = (verticesPerNode * (i - 1)) + (4 * j) + 2; // 2
                    newTriangles[nextTriangle++] = (verticesPerNode * (i - 1)) + (4 * j) + 1; // 1
                    newTriangles[nextTriangle++] = (verticesPerNode * (i - 1)) + (4 * j) + 3; // 3
                    newTriangles[nextTriangle++] = (verticesPerNode * (i - 1)) + (4 * j) + 2; // 2
                }
            }

            newMesh.vertices = newVertices;
            newMesh.triangles = newTriangles;

            newMesh.uv = uvs;

            var myNormals = new Vector3[newMesh.vertexCount];
            for (int p = 0; p < newMesh.vertexCount; p++)
            {
                myNormals[p] = Vector3.up;
            }

            newMesh.normals = myNormals;
            newMesh.RecalculateNormals();

            _pathCollider.sharedMesh = meshFilter.sharedMesh;
            _pathCollider.smoothSphereCollisions = true;

            _pathObject.renderer.enabled = true;
            _pathObject.transform.localScale = new Vector3(1, 1, 1);
        }

        public void FinalizePath()
        {
            _pathObject.transform.localScale = new Vector3(1, 1, 1);
            CreatePath(PathSmooth);
        }

        public Cubic[] CalcNaturalCubic(int n, float[] x)
        {
            var gamma = new float[n + 1];
            var delta = new float[n + 1];
            var d = new float[n + 1];
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

            d[n] = delta[n];

            for (i = n - 1; i >= 0; i--)
            {
                d[i] = delta[i] - gamma[i] * d[i + 1];
            }

            var c = new Cubic[n + 1];
            for (i = 0; i < n; i++)
            {
                c[i] = new Cubic(x[i], d[i], 3 * (x[i + 1] - x[i]) - 2 * d[i] - d[i + 1],
                    2 * (x[i] - x[i + 1]) + d[i] + d[i + 1]);
            }

            return c;
        }
    }
}
