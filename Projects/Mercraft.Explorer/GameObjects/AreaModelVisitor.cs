using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Meshes;
using Mercraft.Explorer.Render;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class AreaModelVisitor: ISceneModelVisitor
    {
        private readonly IEnumerable<IMeshBuilder> _meshBuilders; 
        private readonly IEnumerable<IMeshRenderer> _meshRenderers;

        [Dependency]
        public AreaModelVisitor(IEnumerable<IMeshBuilder> meshBuilders, IEnumerable<IMeshRenderer> meshRenderers)
        {
            _meshBuilders = meshBuilders;
            _meshRenderers = meshRenderers;
        }

        #region ISceneModelVisitor implementation

        public void VisitArea(GeoCoordinate center, GameObject parent, Rule rule, Area area)
        {
            var vertices = PolygonHelper.GetVerticies2D(center, area.Points.ToList());
            vertices = PolygonHelper.SortVertices(vertices);

            var gameObjectName = area.Id;

            var gameObject = new GameObject(gameObjectName);

            // mesh builder
            var meshBuilderName = rule.Evaluate<string>(area, "build");
            var mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = _meshBuilders.First(mb => mb.Name == meshBuilderName).Build(vertices, area, rule);

            // mesh render
            var meshRenderName = rule.Evaluate<string>(area, "render");
            _meshRenderers.First(mr => mr.Name == meshRenderName).Render(gameObject, area, rule);

            gameObject.AddComponent<MeshCollider>();
            gameObject.transform.parent = parent.transform;

        }

        public void VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
        }

        #endregion

    }
}
