using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Interactions;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer
{
    public class GameObjectBuilder: IGameObjectBuilder
    {
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours; 

        [Dependency]
        public GameObjectBuilder(IEnumerable<IModelBuilder> builders, 
            IEnumerable<IModelBehaviour> behaviours)
        {
            _builders = builders;
            _behaviours = behaviours;
        }

        #region IGameObjectBuilder implementation

        public GameObject FromCanvas(GeoCoordinate center, GameObject parent, Rule rule, Canvas canvas)
        {
            var tile = canvas.Tile;
            var material = rule.GetMaterial(canvas);

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "tile";
            quad.transform.position = new Vector3(tile.TileMapCenter.x, 0, tile.TileMapCenter.y);
            quad.transform.transform.localScale = new Vector3(tile.Size, tile.Size, 1);
            quad.transform.transform.Rotate(90, 0, 0);
            quad.renderer.material = material;

            return quad;
        }

        public GameObject FromArea(GeoCoordinate center, GameObject parent, Rule rule, Area area)
        {
            var builder = rule.GetModelBuilder(area, _builders);
            var gameObject = builder.BuildArea(center, rule, area);
            gameObject.name = String.Format("{0} {1}", builder.Name, area);

            var meshFilter =  gameObject.GetComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            gameObject.renderer.material = rule.GetMaterial(area);
            gameObject.renderer.material.color = rule.GetFillColor(area, Color.yellow);
            
            gameObject.transform.parent = parent.transform;

            var collider = gameObject.AddComponent<MeshCollider>();

            collider.sharedMesh = meshFilter.mesh;

            ApplyBehaviour(gameObject, rule, area);

            return gameObject;
        }

        public GameObject FromWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
            var builder = rule.GetModelBuilder(way, _builders);
            var gameObject = builder.BuildWay(center, rule, way);
            gameObject.name = String.Format("{0} {1}", builder.Name, way);
            gameObject.transform.parent = parent.transform;

            ApplyBehaviour(gameObject, rule, way);

            return gameObject;
        }

        #endregion

        private void ApplyBehaviour(GameObject target, Rule rule, Model model)
        {
            var behaviour = rule.GetModelBehaviour(model, _behaviours);
            if(behaviour != null)
                behaviour.Apply(target);
        }

    }
}
