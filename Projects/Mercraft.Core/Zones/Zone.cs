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
        private readonly IGameObjectBuilder _gameObjectBuilder;
        private readonly ITrace _trace;

        public Tile Tile { get; private set; }
        public Stylesheet Stylesheet { get; private set; }

        public Zone(Tile tile,
            Stylesheet stylesheet,
            IGameObjectBuilder gameObjectBuilder,
            ITrace trace)
        {
            Tile = tile;
            Stylesheet = stylesheet;
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

            var canvas = Tile.Scene.Canvas;
            var canvasRule = Stylesheet.GetRule(canvas);
            GameObject canvasObject =
                _gameObjectBuilder.FromCanvas(Tile.RelativeNullPoint, null, canvasRule, canvas);


            // TODO probably, we need to return built game object 
            // to be able to perform cleanup on our side
            BuildAreas(canvasObject, loadedElementIds);
            BuildWays(canvasObject, loadedElementIds);
        }

        private void BuildAreas(GameObject parent, HashSet<long> loadedElementIds)
        {
            foreach (var area in Tile.Scene.Areas)
            {
                if (loadedElementIds.Contains(area.Id))
                    continue;

                var rule = Stylesheet.GetRule(area);
                if (rule.IsApplicable)
                {
                    _gameObjectBuilder.FromArea(Tile.RelativeNullPoint, parent, rule, area);
                    loadedElementIds.Add(area.Id);
                }
                else
                {
                    _trace.Warn(String.Format("No rule for area: {0}, points: {1}", area, area.Points.Length));
                }
            }
        }

        private void BuildWays(GameObject parent, HashSet<long> loadedElementIds)
        {
            foreach (var way in Tile.Scene.Ways)
            {
                if (loadedElementIds.Contains(way.Id))
                    continue;

                var rule = Stylesheet.GetRule(way);
                if (rule.IsApplicable)
                {
                    _gameObjectBuilder.FromWay(Tile.RelativeNullPoint, parent, rule, way);
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
