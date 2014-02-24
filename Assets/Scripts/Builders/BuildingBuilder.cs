
using System.Linq;
using Mercraft.Models;
using Mercraft.Models.Algorithms;
using Mercraft.Models.Scene;
using UnityEngine;

namespace Mercraft.Scene.Builders
{
    /// <summary>
    /// Builds game object which represents building
    /// </summary>
    public class BuildingBuilder : ISceneObjectBuilder<Building>
    {
        private GeoCoordinate _center;
        private float _buildingFloor;
        private float _buildingTop;

        public BuildingBuilder(GeoCoordinate center)
        {
            _center = center;
            _buildingFloor = 0;
            _buildingTop = 40;
        }

        public GameObject Build(string name, Building building)
        {
            var vertices = PolygonHelper.GetVerticies2D(_center, building.Points.ToList());
            return BuildGameObject(name, vertices);
        }

        private GameObject BuildGameObject(string name, Vector2[] verticies2D)
        {
            // NOTE test code: for testing purpose only!
            Debug.Log("try to create mesh..");

            var gameObject = new GameObject(name);
            Mesh mesh = new Mesh();
            mesh.name = "BuildingScript";

            mesh.vertices = PolygonHelper.GetVerticies3D(verticies2D, _buildingTop, _buildingFloor);
            mesh.uv = PolygonHelper.GetUV(verticies2D);
            mesh.triangles = PolygonHelper.GetTriangles(verticies2D);

            mesh.RecalculateNormals();
            var mf = gameObject.AddComponent<MeshFilter>();

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material.shader = Shader.Find("Particles/Additive");
            
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.green);
            tex.Apply();
            renderer.material.mainTexture = tex;
            renderer.material.color = Color.green;

            mf.mesh = mesh;

            gameObject.AddComponent<MeshCollider>();

            Debug.Log("mesh created!");
            return gameObject;
        }
    }
}
