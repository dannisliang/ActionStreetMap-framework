using System;
using System.Collections.Generic;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;

namespace Mercraft.Explorer.Scene
{
    public class ObjectModelBuilder: IModelBuilder
    {
        private readonly HashSet<long> _loadedModelIds;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        public string Name { get { return "gameobject"; }}

        public ObjectModelBuilder(HashSet<long> loadedModelIds,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _loadedModelIds = loadedModelIds;
            _builders = builders;
            _behaviours = behaviours;
        }

        public IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            return ProcessGameObject(tile, rule, area, b => b.BuildArea(tile, rule, area));
        }

        public IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            return ProcessGameObject(tile, rule, way, b => b.BuildWay(tile, rule, way));
        }

        private IGameObject ProcessGameObject(Tile tile, Rule rule, Model model, Func<IModelBuilder, IGameObject> goFunc)
        {
            if (!_loadedModelIds.Contains(model.Id))
            {
                var builder = rule.GetModelBuilder(_builders);
                if (builder == null)
                    return null;
                var gameObjectWrapper = goFunc(builder);

                gameObjectWrapper.Parent = tile.GameObject;

                var behaviour = rule.GetModelBehaviour(_behaviours);
                if (behaviour != null)
                    behaviour.Apply(gameObjectWrapper, model);

                _loadedModelIds.Add(model.Id);

                return gameObjectWrapper;
            }

            return null;
        }
    }
}
