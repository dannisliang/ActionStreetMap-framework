using System;
using Mercraft.Core;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class CanvasModelVisitor : SceneModelVisitor
    {
        public override GameObject VisitCanvas(GeoCoordinate center, GameObject parent, Rule rule, Canvas canvas)
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
    }
}
