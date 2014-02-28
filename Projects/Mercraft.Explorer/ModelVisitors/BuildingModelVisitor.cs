using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Explorer.ModelVisitors
{
    public class BuildingModelVisitor: ISceneModelVisitor
    {
        private readonly float _buildingFloor;
        private readonly float _buildingTop;

        public BuildingModelVisitor()
        {
            _buildingFloor = 0;
            _buildingTop = 3;
        }

        #region ISceneModelVisitor implementation

        public void VisitBuilding(GeoCoordinate center, GameObject parent, Building building)
        {
            var vertices = PolygonHelper.GetVerticies2D(center, building.Points.ToList());
            var name = Guid.NewGuid().ToString();
            BuildGameObject(name, vertices);
        }

        public void VisitRoad(GeoCoordinate center, GameObject parent, Road road)
        {
        }

        #endregion

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
