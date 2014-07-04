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
using Mercraft.Infrastructure.Primitives;
using Mercraft.Models.Terrain;
using Mercraft.Models.Terrain.Roads;
using UnityEngine;

namespace Mercraft.Explorer
{
    public class GameObjectBuilder : IGameObjectBuilder
    {
        private readonly IGameObjectFactory _goFactory;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;

        private List<Tuple<Area, Rule>> _terrainAreas = new List<Tuple<Area, Rule>>();
        private List<Tuple<Way, Rule>> _terrainWays = new List<Tuple<Way, Rule>>();

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

            var terrainSettings = new TerrainSettings()
            {
                CenterPosition = new Vector2(tile.TileMapCenter.X, tile.TileMapCenter.Y),
                TerrainSize = tile.Size,
                SplatPrototypes = rule.GetSplatPrototypes(),
                Areas = _terrainAreas.Select(a => new Mercraft.Models.Terrain.Areas.Area()
                {
                    Points = a.Item1.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray(),
                    ZIndex = a.Item2.GetZIndex(),
                    SplatIndex = a.Item2.GetSplatIndex()
                }).ToArray(),
                Roads = _terrainWays.Select(a => new Road()
                {
                    Points = a.Item1.Points.Select(p => GeoProjection.ToMapCoordinate(center, p)).ToArray(),
                    ZIndex = a.Item2.GetZIndex(),
                    Width = a.Item2.GetWidth(),
                    SplatIndex = a.Item2.GetSplatIndex()
                }).ToArray()
            };

            var terrainBuilder = new TerrainBuilder(terrainSettings);
            return terrainBuilder.Build(parent);
        }

        public IGameObject FromArea(GeoCoordinate center, IGameObject parent, Rule rule, Area area)
        {
            if (rule.IsTerrain())
            {
                _terrainAreas.Add(new Tuple<Area, Rule>(area, rule));
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
            if (rule.IsTerrain())
            {
                _terrainWays.Add(new Tuple<Way, Rule>(way, rule));
                // TODO in future we want to build some special object which will be
                // invisible as it's part of terrain but useful to provide some OSM info
                // which is associated with it. For road it will be some waypoints which
                // help AI to be placed on this road
                return null;
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