using System.Collections.Generic;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class Zone
    {
        private readonly Tile _tile;
        private readonly Stylesheet _stylesheet;
        private readonly IEnumerable<ISceneModelVisitor> _sceneModelVisitors;

        public Zone(Tile tile, 
            Stylesheet stylesheet,
            IEnumerable<ISceneModelVisitor> sceneModelVisitors)
        {
            _tile = tile;
            _stylesheet = stylesheet;
            _sceneModelVisitors = sceneModelVisitors;
        }

        public void Build()
        {
            // TODO refactor this logic

            var canvas = _tile.Scene.Canvas;
            var canvasRule = _stylesheet.GetRule(canvas);
            GameObject canvasObject = null;
            foreach (var sceneModelVisitor in _sceneModelVisitors)
            {
               canvasObject = sceneModelVisitor.VisitCanvas(_tile.RelativeNullPoint, null, canvasRule, canvas);
                if (canvasObject != null) 
                    break;
            }


            foreach (var area in _tile.Scene.Areas)
            {
                var rule = _stylesheet.GetRule(area);
                if (rule != null)
                {
                    foreach (var sceneModelVisitor in _sceneModelVisitors)
                    {

                        // TODO probably, we need to return built game object 
                        // to be able to perform cleanup on our side
                        sceneModelVisitor.VisitArea(_tile.RelativeNullPoint, canvasObject, rule, area);

                    }
                }
            }
        }
    }
}
