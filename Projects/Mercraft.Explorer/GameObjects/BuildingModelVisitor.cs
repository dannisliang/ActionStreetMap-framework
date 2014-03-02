using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Meshes;
using Mercraft.Explorer.Render;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class BuildingModelVisitor: ISceneModelVisitor, IConfigurable
    {
        private PolygonMeshBuilder _meshBuilder;
        private IEnumerable<IMeshRenderer> _meshRenderers;

        private IMeshRenderer _meshRenderer;
        private float _buildingFloor;
        private float _buildingTop;

        [Dependency]
        public BuildingModelVisitor(PolygonMeshBuilder meshBuilder, IEnumerable<IMeshRenderer> meshRenderers)
        {
            _meshBuilder = meshBuilder;
            _meshRenderers = meshRenderers;
        }

        #region ISceneModelVisitor implementation

        public void VisitBuilding(GeoCoordinate center, GameObject parent, Building building)
        {
            var vertices = PolygonHelper.GetVerticies2D(center, building.Points.ToList());
            var name = Guid.NewGuid().ToString();
            BuildGameObject(name, vertices, parent);
        }

        public void VisitRoad(GeoCoordinate center, GameObject parent, Road road)
        {
        }

        #endregion

        private void BuildGameObject(string name, Vector2[] verticies2D, GameObject parent)
        {
            var gameObject = new GameObject(name);
           
            var mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = _meshBuilder.BuildMesh(verticies2D, _buildingTop, _buildingFloor);
           
            // NOTE do we need call this?
            MeshHelper.TangentSolver(gameObject.GetComponent<MeshFilter>().mesh);

            _meshRenderer.Render(gameObject);

            gameObject.AddComponent<MeshCollider>();

            gameObject.transform.parent = parent.transform;
        }

        public void Configure(IConfigSection configSection)
        {
            var renderName = configSection.GetString("render/@name");
            _meshRenderer = _meshRenderers.Single(mr => mr.Name == renderName);

            _buildingFloor = configSection.GetFloat("height/@floor");
            _buildingTop = configSection.GetFloat("height/@top");

            _meshRenderers = null;
        }
    }
}
