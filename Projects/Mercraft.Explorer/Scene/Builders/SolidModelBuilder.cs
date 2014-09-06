using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    public class SolidModelBuilder : ModelBuilder
    {
        public override string Name
        {
            get { return "solid"; }
        }

        [Dependency]
        public SolidModelBuilder(IGameObjectFactory gameObjectFactory)
            : base(gameObjectFactory)
        {
        }

        public override IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            base.BuildArea(tile, rule, area);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Solid {0}", area));
            BuildModel(tile.RelativeNullPoint, gameObjectWrapper, rule, area.Points.ToList());
            return gameObjectWrapper;
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            base.BuildWay(tile, rule, way);
            IGameObject gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("Solid {0}", way));
            BuildModel(tile.RelativeNullPoint, gameObjectWrapper, rule, way.Points.ToList());
            return gameObjectWrapper;
        }

        private void BuildModel(GeoCoordinate center, IGameObject gameObject, Rule rule,
            IList<GeoCoordinate> coordinates)
        {
            var height = rule.GetHeight();

            var floor = rule.GetZIndex();
            var top = floor + height;

            var verticies = PolygonHelper.GetVerticies2D(center, coordinates);

            var mesh = new Mesh();
            mesh.vertices = verticies.GetVerticies3D(top, floor);
            //mesh.uv = verticies.GetUV();
            mesh.triangles = PolygonHelper.GetTriangles3D(verticies);

            var unityGameObject = gameObject.GetComponent<GameObject>();

            var meshFilter = unityGameObject.AddComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();

            unityGameObject.AddComponent<MeshRenderer>();
            unityGameObject.renderer.material = rule.GetMaterial();
            unityGameObject.renderer.material.color = rule.GetFillColor();
        }
    }
}