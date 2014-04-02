using System;
using System.Collections.Generic;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class Zone
    {
        private readonly Tile _tile;
        private readonly Stylesheet _stylesheet;
        private readonly IGameObjectBuilder _gameObjectBuilder;

        private readonly ITrace _trace;

        public Zone(Tile tile,
            Stylesheet stylesheet,
            IGameObjectBuilder gameObjectBuilder,
            ITrace trace)
        {
            _tile = tile;
            _stylesheet = stylesheet;
            _gameObjectBuilder = gameObjectBuilder;
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
            GameObject canvasObject =  
                _gameObjectBuilder.FromCanvas(_tile.RelativeNullPoint, null, canvasRule, canvas);


            // TODO probably, we need to return built game object 
            // to be able to perform cleanup on our side
            BuildAreas(canvasObject, loadedElementIds);
            BuildWays(canvasObject, loadedElementIds);
        }

        private void BuildAreas(GameObject parent, HashSet<long> loadedElementIds)
        {
            foreach (var area in _tile.Scene.Areas)
            {
                if (loadedElementIds.Contains(area.Id))
                    continue;

                var rule = _stylesheet.GetRule(area);
                if (rule.IsApplicable)
                {
                    _trace.Info(String.Format("Try to build.. {0}", area));
                   _gameObjectBuilder.FromArea(_tile.RelativeNullPoint, parent, rule, area);
                    loadedElementIds.Add(area.Id);
                    _trace.Info(String.Format("Done.. {0}", area));
                }
                else
                {
                    _trace.Warn(String.Format("No rule for area: {0}, points: {1}", area, area.Points.Length));
                }
            }
        }

        private void BuildWays(GameObject parent, HashSet<long> loadedElementIds)
        {
            foreach (var way in _tile.Scene.Ways)
            {
                if (loadedElementIds.Contains(way.Id))
                    continue;

                var rule = _stylesheet.GetRule(way);
                if (rule.IsApplicable)
                {
                    _trace.Info(String.Format("Try to build way.. {0}", way));
                     _gameObjectBuilder.FromWay(_tile.RelativeNullPoint, parent, rule, way);
                    loadedElementIds.Add(way.Id);
                    _trace.Info(String.Format("Done.. {0}", way));
                }
                else
                {
                    _trace.Warn(String.Format("No rule for way: {0}, points: {1}", way, way.Points.Length));
                }
            }
        }
    }
}
