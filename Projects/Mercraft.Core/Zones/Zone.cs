using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Core.Zones
{
    /// <summary>
    ///     Represents square with created game objects
    /// </summary>
    public class Zone
    {
        private readonly ISceneVisitor _sceneVisitor;
        private readonly IGameObjectFactory _goFactory;
        private readonly ITrace _trace;

        public Tile Tile { get; private set; }
        public Stylesheet Stylesheet { get; private set; }

        public Zone(Tile tile,
            Stylesheet stylesheet,
            IGameObjectFactory gameObjectFactory,
            ISceneVisitor sceneVisitor,
            ITrace trace)
        {
            Tile = tile;
            Stylesheet = stylesheet;
            _goFactory = gameObjectFactory;
            _sceneVisitor = sceneVisitor;
            _trace = trace;
        }

        /// <summary>
        ///     Builds game objects
        /// </summary>
        /// <param name="loadedElementIds">Contains ids of previously loaded elements. Used to prevent duplicates</param>
        public void Build(HashSet<long> loadedElementIds)
        {
            // NOTE Dispose scene to release memory
            using (Tile.Scene)
            {
                Tile.GameObject = _goFactory.CreateNew("tile");

                _sceneVisitor.Prepare(Tile.Scene, Stylesheet);

                BuildModel(Tile.Scene.Areas.ToList(), loadedElementIds, (area, rule, visited) =>
                    _sceneVisitor.VisitArea(Tile, rule, area, visited));

                BuildModel(Tile.Scene.Ways.ToList(), loadedElementIds, (way, rule, visited) =>
                    _sceneVisitor.VisitWay(Tile, rule, way, visited));

                var canvas = Tile.Scene.Canvas;
                var canvasRule = Stylesheet.GetRule(canvas, false);
                _sceneVisitor.VisitCanvas(Tile, canvasRule, canvas, false);

                _sceneVisitor.Finalize(Tile.Scene);
            }
        }

        private void BuildModel<T>(IEnumerable<T> models, HashSet<long> loadedElementIds,
            Func<T, Rule, bool, SceneVisitResult> builder) where T : Model
        {
            foreach (T model in models)
            {
                try
                {
                   // if (loadedElementIds.Contains(model.Id))
                   //     continue;

                    var rule = Stylesheet.GetRule(model);
                    if (rule.IsApplicable)
                    {
                        var result = builder(model, rule, loadedElementIds.Contains(model.Id));
                        if (result == SceneVisitResult.Completed && !loadedElementIds.Contains(model.Id))
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