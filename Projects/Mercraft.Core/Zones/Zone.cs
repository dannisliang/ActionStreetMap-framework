using System.Collections.Generic;
using System.Linq;
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

            // visit canvas
            foreach (var sceneModelVisitor in _sceneModelVisitors)
            {
               canvasObject = sceneModelVisitor.VisitCanvas(_tile.RelativeNullPoint, null, canvasRule, canvas);
                if (canvasObject != null) 
                    break;
            }

            // TODO probably, we need to return built game object 
            // to be able to perform cleanup on our side

            // visit areas
            foreach (var area in _tile.Scene.Areas)
            {
                var rule = _stylesheet.GetRule(area);
                if (rule != null)
                {
                    foreach (var sceneModelVisitor in _sceneModelVisitors)
                    {
                        sceneModelVisitor.VisitArea(_tile.RelativeNullPoint, canvasObject, rule, area);
                    }
                }
            }

            // visit ways
            foreach (var way in _tile.Scene.Ways)
            {
                var rule = _stylesheet.GetRule(way);
                if (rule != null)
                {
                    foreach (var sceneModelVisitor in _sceneModelVisitors)
                    {
                        sceneModelVisitor.VisitWay(_tile.RelativeNullPoint, canvasObject, rule, way);
                    }
                }
            }
        }
    }
}
