using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Explorer.Helpers;
using Mercraft.Models.Areas;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer
{
    public class GameObjectBuilder : IGameObjectBuilder
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        private List<AreaSettings> _areas = new List<AreaSettings>();
        private List<RoadSettings> _roads = new List<RoadSettings>();

        public GameObjectBuilder(IGameObjectFactory goFactory,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours)
        {
            _goFactory = goFactory;
            _builders = builders.ToList();
            _behaviours = behaviours.ToList();
        }

        #region IGameObjectBuilder implementation

        public IGameObject CreateTileHolder()
        {
            return _goFactory.CreateNew("tile");
        }

        public IGameObject FromCanvas(GeoCoordinate center, IGameObject parent, Rule rule, Canvas canvas)
        {
            var tile = canvas.Tile;
            var terrainBuilder = new TerrainBuilder(new TerrainSettings()
            {
                AlphaMapSize = rule.GetAlphaMapSize(),
                CenterPosition = new Vector2(tile.TileMapCenter.X, tile.TileMapCenter.Y),
                TerrainSize = tile.Size,
                SplatPrototypes = rule.GetSplatPrototypes(),
                Areas = _areas,
                Roads = _roads
            });

            // NOTE not ideal solution to make class ready for next request
            _areas = new List<AreaSettings>();
            _roads = new List<RoadSettings>();

            return terrainBuilder.Build(parent);
        }

        public IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            if (rule.IsTerrain())
            {
                _areas.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    SplatIndex = rule.GetSplatIndex(),
                    Points = area.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray()
                });
                // TODO in future we want to build some special object which will be
                // invisible as it's part of terrain but useful to provide some OSM info
                // which is associated with it
                return null;
            }

            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildArea(center, rule, area);
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();
            gameObject.name = String.Format("{0} {1}", builder.Name, area);

            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;
            ApplyBehaviour(gameObjectWrapper, rule, area);

            return gameObjectWrapper;
        }

        public IGameObject FromWay(GeoCoordinate center, IGameObject parent, Rule rule, Way way)
        {
            // NOTE Road should be processed with Terrain as it has dependencies on:
            // 1. its heightmap (so far not important as we have flat map)
            // 2. we should join roads (important)
            if (rule.IsRoad())
            {
                var roadGameObject = _goFactory.CreateNew(way.ToString());
                _roads.Add(new RoadSettings()
                {
                    Width = (int) Math.Round(rule.GetWidth() / 2),
                    TargetObject = roadGameObject,
                    Points = way.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray()
                });
                // this game object should be initialized inside of TerrainBuilder's logic
                return roadGameObject;
            }

            var builder = rule.GetModelBuilder(_builders);
            var gameObjectWrapper = builder.BuildWay(center, rule, way);
            var gameObject = gameObjectWrapper.GetComponent<GameObject>();
            gameObject.name = String.Format("{0} {1}", builder.Name, way);
            gameObject.transform.parent = parent.GetComponent<GameObject>().transform;

            ApplyBehaviour(gameObjectWrapper, rule, way);

            return gameObjectWrapper;
        }

        #endregion

        private void ApplyBehaviour(IGameObject target, Rule rule, Model model)
        {
            var behaviour = rule.GetModelBehaviour(_behaviours);
            if (behaviour != null)
                behaviour.Apply(target, model);
        }
    }
}