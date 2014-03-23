using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class Zone
    {
        private readonly Tile _tile;
        private readonly Stylesheet _stylesheet;
        private readonly IEnumerable<ISceneModelVisitor> _sceneModelVisitors;

        private readonly ITrace _trace;

        public Zone(Tile tile, 
            Stylesheet stylesheet,
            IEnumerable<ISceneModelVisitor> sceneModelVisitors, ITrace trace)
        {
            _tile = tile;
            _stylesheet = stylesheet;
            _sceneModelVisitors = sceneModelVisitors;
            _trace = trace;
        }

        /// <summary>
        /// Builds game objects
        /// </summary>
        /// <param name="loadedElementIds">Contains ids of previously loaded elements. Used to prevent duplicates</param>
        public void Build(HashSet<long> loadedElementIds)
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
                if(loadedElementIds.Contains(area.Id))
                    continue;
                
                var rule = _stylesheet.GetRule(area);
                if (rule != null)
                {
                    foreach (var sceneModelVisitor in _sceneModelVisitors)
                    {
                        sceneModelVisitor.VisitArea(_tile.RelativeNullPoint, canvasObject, rule, area);
                    }
                    loadedElementIds.Add(area.Id);
                }
                else
                {
                    _trace.Warn(String.Format("No rule for area: {0}, points: {1}", area, area.Points.Length));
                }
            }

            // visit ways
            foreach (var way in _tile.Scene.Ways)
            {
                if (loadedElementIds.Contains(way.Id))
                    continue;

                var rule = _stylesheet.GetRule(way);
                if (rule != null)
                {
                    foreach (var sceneModelVisitor in _sceneModelVisitors)
                    {
                        sceneModelVisitor.VisitWay(_tile.RelativeNullPoint, canvasObject, rule, way);
                    }
                    loadedElementIds.Add(way.Id);
                }
                else
                {
                    _trace.Warn(String.Format("No rule for way: {0}, points: {1}", way, way.Points.Length));
                }
            }
        }
    }
}
