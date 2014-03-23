using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Builders;
using Mercraft.Explorer.Helpers;
using Mercraft.Infrastructure.Dependencies;
using UnityEngine;

namespace Mercraft.Explorer
{
    public class SceneModelVisitor: ISceneModelVisitor
    {
        private readonly IEnumerable<IModelBuilder> _builders; 

        [Dependency]
        public SceneModelVisitor(IEnumerable<IModelBuilder> builders)
        {
            _builders = builders;
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
            var builder = ProcessAndGetBuilder(gameObject, center, parent, rule, area, Color.yellow);
            builder.BuildArea(center, gameObject, rule, area);

            return gameObject;
        }

        public GameObject VisitWay(GeoCoordinate center, GameObject parent, Rule rule, Way way)
        {
            var gameObject = new GameObject();
            var builder = ProcessAndGetBuilder(gameObject, center, parent, rule, way, Color.red);
            builder.BuildWay(center, gameObject, rule, way);

            return gameObject;
        }

        #endregion

        private IModelBuilder ProcessAndGetBuilder(GameObject gameObject, GeoCoordinate center, GameObject parent, Rule rule, Model model, Color defaultColor)
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshCollider>();
            gameObject.renderer.material = rule.GetMaterial(model);
            gameObject.renderer.material.color = rule.GetFillColor(model, defaultColor);
            gameObject.transform.parent = parent.transform;

            return rule.GetModelBuilder(model, _builders);
        }

    }
}
