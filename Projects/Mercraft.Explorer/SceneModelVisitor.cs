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
    public class SceneModelVisitor: ISceneModelVisitor
    {
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours; 

        [Dependency]
        public SceneModelVisitor(IEnumerable<IModelBuilder> builders, 
            IEnumerable<IModelBehaviour> behaviours)
        {
            _builders = builders;
            _behaviours = behaviours;
        }

        #region ISceneModelVisitor implementation

        public GameObject VisitCanvas(GeoCoordinate center, GameObject parent, Rule rule, Canvas canvas)
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

        public GameObject VisitArea(GeoCoordinate center, GameObject parent, Rule rule, Area area)
        {
            var gameObject = new GameObject();

            var meshFilter =  gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            gameObject.renderer.material = rule.GetMaterial(area);
            gameObject.renderer.material.color = rule.GetFillColor(area, Color.yellow);
            
            gameObject.transform.parent = parent.transform;

            var collider = gameObject.AddComponent<MeshCollider>();

            var builder = rule.GetModelBuilder(area, _builders);
            builder.BuildArea(center, gameObject, rule, area);

            collider.sharedMesh = meshFilter.mesh;

            ApplyBehaviour(gameObject, rule, area);

            return gameObject;
        }

        public GameObject VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
            var gameObject = new GameObject();
            var builder = rule.GetModelBuilder(way, _builders);
            builder.BuildWay(center, gameObject, rule, way);
            
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
