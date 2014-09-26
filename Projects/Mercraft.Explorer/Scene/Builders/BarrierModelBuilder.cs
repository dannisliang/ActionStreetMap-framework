using System;
using System.Collections.Generic;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Models.Geometry.ThickLine;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Explorer.Scene.Builders
{
    public class BarrierModelBuilder: ModelBuilder
    {
        private readonly IResourceProvider _resourceProvider;
        protected readonly DimenLineBuilder DimenLineBuilder = new DimenLineBuilder(2);
        private readonly List<LineElement> _lines = new List<LineElement>(1);
        public override string Name
        {
            get { return "barrier"; }
        }

        [Dependency]
        public BarrierModelBuilder(WorldManager worldManager, IGameObjectFactory gameObjectFactory, IResourceProvider resourceProvider) : 
            base(worldManager, gameObjectFactory)
        {
            _resourceProvider = resourceProvider;
        }

        public override IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            if (way.Points.Length < 2)
            {
                Trace.Warn(ErrorStrings.InvalidPolyline);
                return null;
            }

            var gameObjectWrapper = GameObjectFactory.CreateNew(String.Format("{0} {1}", Name, way));

            var points = PolygonHelper.GetVerticies3D(tile.RelativeNullPoint, tile.HeightMap, way.Points);
            
            // reuse lines
            _lines.Clear();
            _lines.Add(new LineElement(points, rule.GetWidth()));

            DimenLineBuilder.Height = rule.GetHeight();
            DimenLineBuilder.Build(tile.HeightMap, _lines, 
                (p, t, u) => BuildObject(gameObjectWrapper, rule, p, t, u));
            _lines.Clear();
            return gameObjectWrapper;
        }

        protected virtual void BuildObject(IGameObject gameObjectWrapper, Rule rule,
            List<Vector3> p, List<int> t, List<Vector2> u)
        {
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();

            Mesh mesh = new Mesh();
            mesh.vertices = p.ToArray();
            mesh.triangles = t.ToArray();
            mesh.uv = u.ToArray();
            mesh.RecalculateNormals();

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = rule.GetMaterial(_resourceProvider);
            renderer.material.mainTexture = rule.GetTexture(_resourceProvider);
        }
    }
}
