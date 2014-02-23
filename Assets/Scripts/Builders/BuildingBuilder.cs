
using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Models;
using Mercraft.Models.Algorithms;
using Mercraft.Models.Scene;
using UnityEngine;

namespace Mercraft.Scene.Builders
{
    public class BuildingBuilder
    {
        private readonly MapPoint _center;

        private float _buildingFloor;
        private float _buildingTop;

        public BuildingBuilder(MapPoint center)
        {
            _center = center;
            _buildingFloor = 0;
            _buildingTop = 40;
        }

        public void Build(string name, Building building)
        {
            var vertices = GetMapCoordinates(building.Points.ToList());
            BuildGameObject(name, vertices);
        }

        private Vector2[] GetMapCoordinates(IList<MapPoint> geoCoordinates)
        {
            var length = geoCoordinates.Count;

            if (geoCoordinates[0] == geoCoordinates[length - 1]) length--;

            return geoCoordinates
                .Select(g => GeoProjection.ToMapCoordinates(_center, g))
                .Take(length).ToArray();
        }

        private Vector3[] GetVerticies3D(Vector2[] verticies2D)
        {
            var length = verticies2D.Length;
            var verticies3D = new Vector3[length * 2];
            for (int i = 0; i < length; i++)
            {
                verticies3D[i] = new Vector3(verticies2D[i].x, _buildingFloor, verticies2D[i].y);
                verticies3D[i + length] = new Vector3(verticies2D[i].x, _buildingTop, verticies2D[i].y);
            }

            return verticies3D;
        }

        private int[] GetTriangles(Vector2[] verticies2D)
        {
            var verticiesLength = verticies2D.Length;
            var indecies = PolygonTriangulation.GetTriangles(verticies2D);
            var length = indecies.Length;

            // add top
            Array.Resize(ref indecies, length*2);
            for (var i = 0; i < length; i++)
            {
                indecies[i + length] = indecies[i] + verticiesLength;
            }

            // process square faces
            var oldIndeciesLength = indecies.Length;
            var faceTriangleCount = verticiesLength*6;
            Array.Resize(ref indecies, oldIndeciesLength + faceTriangleCount);

            int j = 0;
            for (var i = 0; i < verticiesLength - 1; i++)
            {
                indecies[i + oldIndeciesLength + j] = i;
                indecies[i + oldIndeciesLength + ++j] = i + 1;
                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength;

                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength;
                indecies[i + oldIndeciesLength + ++j] = i + 1;
                indecies[i + oldIndeciesLength + ++j] = i + verticiesLength + 1;
            }

            return indecies;
        }

        private Vector2[] GetUV(Vector2[] verticies2D)
        {
            var length = verticies2D.Length;
            var uvs = new Vector2[length * 2];

            for (int i = 0; i < length; i++)
            {
                uvs[i] = new Vector2(verticies2D[i].x, verticies2D[i].y);
                uvs[i + length] = new Vector2(verticies2D[i].x, verticies2D[i].y);
            }

            return uvs;
        }

        private void BuildGameObject(string name, Vector2[] verticies2D)
        {
            Debug.Log("try to create mesh..");

            
            var OurNewMesh = new GameObject(name);
            Mesh mesh = new Mesh();
            mesh.name = "MyScripted";
            
            mesh.vertices = GetVerticies3D(verticies2D);
            mesh.uv = GetUV(verticies2D);
            mesh.triangles = GetTriangles(verticies2D);

            mesh.RecalculateNormals();
            var mf = OurNewMesh.AddComponent<MeshFilter>();

            var renderer = OurNewMesh.AddComponent<MeshRenderer>();
            renderer.material.shader = Shader.Find("Particles/Additive");
            
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.green);
            tex.Apply();
            renderer.material.mainTexture = tex;
            renderer.material.color = Color.green;

            mf.mesh = mesh;

            OurNewMesh.AddComponent<MeshCollider>();

            Debug.Log("mesh created!");
        }
    }
}
