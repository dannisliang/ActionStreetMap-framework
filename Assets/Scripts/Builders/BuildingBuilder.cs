
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

        public BuildingBuilder(MapPoint center)
        {
            _center = center;
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

        private void BuildGameObject(string name, Vector2[] verticies)
        {
            Debug.Log("try to create mesh..");

            Vector3[] verticies3D = new Vector3[verticies.Length];
            for (int ii1 = 0; ii1 < verticies.Length; ii1++)
            {
                verticies3D[ii1] = new Vector3(verticies[ii1].x, 0, verticies[ii1].y);
            }
            var OurNewMesh = new GameObject(name);
            Mesh mesh = new Mesh();
            mesh.name = "MyScripted";
            mesh.vertices = verticies3D;
            mesh.uv = verticies;

            mesh.triangles = PolygonTriangulation.GetTriangles(verticies);

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

            Debug.Log("mesh created!");
        }
    }
}
