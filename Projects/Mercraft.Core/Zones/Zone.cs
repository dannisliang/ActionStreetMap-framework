using System;
using System.Collections.Generic;
using System.Net.Configuration;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
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

            BuildModel(Tile.Scene.Areas, loadedElementIds, (area, rule) =>
                _gameObjectBuilder.FromArea(Tile.RelativeNullPoint, canvasObject, rule, area));

            BuildModel(Tile.Scene.Ways, loadedElementIds, (way, rule) =>
                _gameObjectBuilder.FromWay(Tile.RelativeNullPoint, canvasObject, rule, way));
        }

        private void BuildModel<T>(IEnumerable<T> models, HashSet<long> loadedElementIds, 
            Func<T, Rule, GameObject> builder) where T: Model
        {
            foreach (T model in models)
            {
                try
                {
                    if (loadedElementIds.Contains(model.Id))
                        continue;

                    var rule = Stylesheet.GetRule(model);
                    if (rule.IsApplicable)
                    {
                        // TODO probably, we need to return built game object 
                        // to be able to perform cleanup on our side
                        builder(model, rule);
                        loadedElementIds.Add(model.Id);
                    }
                    else
                    {
                        // TODO move Points properties to Model?
                        var points = (model is Area) ? (model as Area).Points : (model as Way).Points;
                        _trace.Warn(String.Format("No rule for model: {0}, points: {1}", model, points.Length));
                    }
                }
                catch (Exception ex)
                {
                    _trace.Error(String.Format("Unable to build model: {0}", model), ex);
                    throw;
                }
            }
        }
    }
}
