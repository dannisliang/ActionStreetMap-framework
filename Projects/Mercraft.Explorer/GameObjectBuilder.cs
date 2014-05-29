using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Interactions;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer
{
    public class GameObjectBuilder : IGameObjectBuilder
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        [Dependency]
        public GameObjectBuilder(IGameObjectFactory goFactory,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _goFactory = goFactory;
            _builders = builders;
            _behaviours = behaviours;
        }

        #region IGameObjectBuilder implementation

        public IGameObject FromCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas)
        {
            var tile = canvas.Tile;
            var material = rule.GetMaterial();

            var gameObjectWrapper = _goFactory.CreatePrimitive("", PrimitiveType.Quad);
            var quad = gameObjectWrapper.GetComponent<GameObject>();
            quad.name = "tile";
            quad.transform.position = new Vector3(tile.TileMapCenter.x, 0, tile.TileMapCenter.y);
            quad.transform.transform.localScale = new Vector3(tile.Size, tile.Size, 1);
            quad.transform.transform.Rotate(90, 0, 0);
            quad.renderer.material = material;

            return gameObjectWrapper;
        }

        public IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildArea(center, rule, area);
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();
            gameObject.name = String.Format("{0} {1}", builder.Name, area);


            /*var meshFilter = gameObject.GetComponent<MeshFilter>();

            var collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.mesh;*/

            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            ApplyBehaviour(gameObjectWrapper, rule);

            return gameObjectWrapper;
        }

        public IGameObject FromWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way)
        {
            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildWay(center, rule, way);
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();
            gameObject.name = String.Format("{0} {1}", builder.Name, way);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;

            ApplyBehaviour(gameObjectWrapper, rule);

            return gameObjectWrapper;
        }

        #endregion

        private void ApplyBehaviour(IGameObject target, Rule rule)
        {
            var behaviour = rule.GetModelBehaviour(_behaviours);
            if (behaviour != null)
                behaviour.Apply(target);
        }
    }
}