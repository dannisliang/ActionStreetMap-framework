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
        private const string MeshRenderNameKey = @"render/@name";
        private const string BuildingLevelHeightKey = @"height/@level";

        private PolygonMeshBuilder _meshBuilder;
        private IEnumerable<IMeshRenderer> _meshRenderers;

        private IMeshRenderer _meshRenderer;

        private float _levelHeight;

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

            // TODO Sort in place!
            vertices = PolygonHelper.SortVertices(vertices);

            var name = Guid.NewGuid().ToString();

            // TODO floor should be calculated using parent object
            var floor = 1;

            // TODO use height if defined?
            var top = floor + building.LevelCount * _levelHeight;

            BuildGameObject(name, vertices, parent, top, floor);
        }

        public void VisitRoad(GeoCoordinate center, GameObject parent, Road road)
        {
        }

        #endregion

        private void BuildGameObject(string name, Vector2[] verticies2D, GameObject parent, float top, float floor)
        {
            var gameObject = new GameObject(name);
          
            var mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = _meshBuilder.BuildMesh(verticies2D, top, floor);
            _meshRenderer.Render(gameObject);

            gameObject.AddComponent<MeshCollider>();

            gameObject.transform.parent = parent.transform;
        }

        public void Configure(IConfigSection configSection)
        {
            var renderName = configSection.GetString(MeshRenderNameKey);
            _meshRenderer = _meshRenderers.Single(mr => mr.Name == renderName);

            _levelHeight = configSection.GetFloat(BuildingLevelHeightKey);

            _meshRenderers = null;
        }
    }
}
