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
    /// Represents square with created game objects
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
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours,
            ITrace trace)
        {
            Tile = tile;
            Stylesheet = stylesheet;
            _goFactory = gameObjectFactory;
            _sceneVisitor = gameObjectFactory.GetBuilder(builders, behaviours);
            _trace = trace;
        }

        /// <summary>
        /// Builds game objects
        /// </summary>
        /// <param name="loadedElementIds">Contains ids of previously loaded elements. Used to prevent duplicates</param>
        public void Build(HashSet<long> loadedElementIds)
        {
            // NOTE Dispose scene to release memory
            using (Tile.Scene)
            {
                var tileHolder = _goFactory.CreateNew("tile");

                BuildModel(Tile.Scene.Areas.ToList(), loadedElementIds, (area, rule) =>
                    _sceneVisitor.VisitArea(Tile.RelativeNullPoint, tileHolder, rule, area));

                BuildModel(Tile.Scene.Ways.ToList(), loadedElementIds, (way, rule) =>
                    _sceneVisitor.VisitWay(Tile.RelativeNullPoint, tileHolder, rule, way));

                var canvas = Tile.Scene.Canvas;
                var canvasRule = Stylesheet.GetRule(canvas, false);
                _sceneVisitor.VisitCanvas(Tile.RelativeNullPoint, tileHolder, canvasRule, canvas);
            }
        }

        private void BuildModel<T>(IEnumerable<T> models, HashSet<long> loadedElementIds,
            Func<T, Rule, bool> builder) where T : Model
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
                        var isCreated = builder(model, rule);
                        if (isCreated)
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
