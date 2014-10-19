using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Details;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Scene
{
    public class TileModelLoader : IModelVisitor, IModelBuilder
    {
        private readonly IHeightMapProvider _heighMapProvider;
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IRoadBuilder _roadBuilder;
        private readonly IObjectPool _objectPool;
        private readonly IModelBuilder[] _builders;
        private readonly IModelBehaviour[] _behaviours;
        private readonly IGameObjectFactory _gameObjectFactory;
        private readonly IThemeProvider _themeProvider;
        private readonly Stylesheet _stylesheet;

        private Tile _tile;

        // NOTE these objects should be processed either as part of terrain or together
        private readonly List<AreaSettings> _areas = new List<AreaSettings>();
        private readonly List<AreaSettings> _elevations = new List<AreaSettings>();
        private readonly List<RoadElement> _roadElements = new List<RoadElement>();
        private readonly List<TreeDetail> _trees = new List<TreeDetail>();

        [Dependency]
        public TileModelLoader(IGameObjectFactory gameObjectFactory, IThemeProvider themeProvider,
            IHeightMapProvider heighMapProvider, ITerrainBuilder terrainBuilder,
            IRoadBuilder roadBuilder, IStylesheetProvider stylesheetProvider,
            IEnumerable<IModelBuilder> builders, IEnumerable<IModelBehaviour> behaviours,
            IObjectPool objectPool)
        {
            _heighMapProvider = heighMapProvider;
            _terrainBuilder = terrainBuilder;
            _roadBuilder = roadBuilder;
            _objectPool = objectPool;
            _builders = builders.ToArray();
            _behaviours = behaviours.ToArray();
            _gameObjectFactory = gameObjectFactory;
            _themeProvider = themeProvider;
            _stylesheet = stylesheetProvider.Get();
        }

        #region IModelVisitor

        public void VisitTile(Tile tile)
        {
            _tile = tile;
            _tile.GameObject = _gameObjectFactory.CreateNew("tile");
        }

        public void VisitArea(Area area)
        {
            var rule = _stylesheet.GetModelRule(area);
            if (ShouldUseBuilder(rule, area))
            {
                BuildArea(_tile, rule, area);

                var modelBuilder = rule.GetModelBuilder(_builders);
                if (modelBuilder != null)
                {
                    var gameObject = modelBuilder.BuildArea(_tile, rule, area);
                    AttachExtras(gameObject, rule, area);
                }

            }
            _objectPool.Store(area.Points);
            _stylesheet.StoreRule(rule);
        }

        public void VisitWay(Way way)
        {
            var rule = _stylesheet.GetModelRule(way);
            if (ShouldUseBuilder(rule, way))
            {
                BuildWay(_tile, rule, way);

                var modelBuilder = rule.GetModelBuilder(_builders);
                if (modelBuilder != null)
                {
                    var gameObject = modelBuilder.BuildWay(_tile, rule, way);
                    AttachExtras(gameObject, rule, way);
                }
            }
            _objectPool.Store(way.Points);
            _stylesheet.StoreRule(rule);
        }

        public void VisitNode(Node node)
        {
            var rule = _stylesheet.GetModelRule(node);
            if (ShouldUseBuilder(rule, node))
            {
                BuildNode(_tile, rule, node);
                var modelBuilder = rule.GetModelBuilder(_builders);
                if (modelBuilder != null)
                {
                    var gameObject = modelBuilder.BuildNode(_tile, rule, node);
                    AttachExtras(gameObject, rule, node);
                }
            }
            _stylesheet.StoreRule(rule);
        }

        public void VisitCanvas(Canvas canvas)
        {
            var rule = _stylesheet.GetCanvasRule(canvas);

            // TODO this should be done by road composer
            var roads = _roadElements.Select(re => new Road
            {
                Elements = new List<RoadElement> {re},
                GameObject = _gameObjectFactory.CreateNew(String.Format("road [{0}] {1}/ ", re.Id, re.Address), _tile.GameObject),
            }).ToList();

            if (_tile.HeightMap.IsFlat)
                _tile.HeightMap.MaxElevation = rule.GetHeight();

            _terrainBuilder.Build(_tile.GameObject, new TerrainSettings
            {
                Tile = _tile,
                Resolution = rule.GetResolution(),
                CenterPosition = new Vector2(_tile.MapCenter.X, _tile.MapCenter.Y),
                CornerPosition = new Vector2(_tile.BottomLeft.X, _tile.BottomLeft.Y),
                PixelMapError = rule.GetPixelMapError(),
                ZIndex = rule.GetZIndex(),
                SplatParams = rule.GetSplatParams(),
                DetailParams = rule.GetDetailParams(),
                Areas = _areas,
                Elevations = _elevations,
                Trees = _trees,
                Roads = roads,
                RoadBuilder = _roadBuilder,
                RoadStyleProvider = _themeProvider.Get().GetStyleProvider<IRoadStyleProvider>()
            });

            // Cleanup
            // return lists to object pool
            foreach (var area in _areas)
                _objectPool.Store(area.Points);
            foreach (var elevation in _elevations)
                _objectPool.Store(elevation.Points);
            // NOTE do not return road element's points back to store as they will be used in future

            // clear collections to reuse
            _areas.Clear();
            _elevations.Clear();
            _roadElements.Clear();
            _trees.Clear();
            _heighMapProvider.Store(_tile.HeightMap);
            _tile.HeightMap = null;

            _objectPool.Shrink();
        }

        private bool ShouldUseBuilder(Rule rule, Model model)
        {
            if (!rule.IsApplicable)
                return false;

            if (rule.IsSkipped())
            {
                CreateSkipped(_tile.GameObject, model);
                return false;
            }

            return true;
        }

        private void CreateSkipped(IGameObject parent, Model model)
        {
            // TODO this is useful only for debug, in release we should avoid creation of 
            // additional objects due to performance reason
            var skippedGameObject = _gameObjectFactory.CreateNew(String.Format("skip {0}", model));
            skippedGameObject.Parent = parent;
        }

        private void AttachExtras(IGameObject gameObject, Rule rule, Model model)
        {
            if (gameObject != null)
            {
                gameObject.Parent = _tile.GameObject;
                var behaviour = rule.GetModelBehaviour(_behaviours);
                if (behaviour != null)
                    behaviour.Apply(gameObject, model);
            }
        }

        #endregion

        #region IModelBuilder 

        public string Name
        {
            get { return "tile"; }
        }

        public IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            if (rule.IsTerrain())
            {
                var points = _objectPool.NewList<MapPoint>();
                PointHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points, points);
                _areas.Add(new AreaSettings
                {
                    ZIndex = rule.GetZIndex(),
                    SplatIndex = rule.GetSplatIndex(),
                    DetailIndex = rule.GetTerrainDetailIndex(),
                    Points = points
                });
            }

            if (rule.IsElevation())
            {
                var points = _objectPool.NewList<MapPoint>();
                PointHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points, points);
                _elevations.Add(new AreaSettings
                {
                    ZIndex = rule.GetZIndex(),
                    Points = points
                });
            }
            return null;
        }

        public IGameObject BuildWay(Tile tile, Rule rule, Way way)
        {
            if (rule.IsRoad())
            {
                // road should be processed in one place: it's better to collect all 
                // roads and create connected road network
                var points = _objectPool.NewList<MapPoint>();
                PointHelper.FillHeight(tile.RelativeNullPoint, tile.HeightMap, way.Points, points, way.Points.Count);
                _roadElements.Add(new RoadElement
                {
                    Id = way.Id,
                    Address = AddressExtractor.Extract(way.Tags),
                    Width = (int) Math.Round(rule.GetWidth()/2),
                    ZIndex = rule.GetZIndex(),
                    Points = points
                });
            }

            // flat road can be rendered fully for cross-tile case, but not for elevation
            return null;
        }

        public IGameObject BuildNode(Tile tile, Rule rule, Node node)
        {
            if (rule.IsTree())
            {
                // TODO extract this method
                var mapPoint = GeoProjection.ToMapCoordinate(tile.RelativeNullPoint, node.Point);
                mapPoint.Elevation = tile.HeightMap.LookupHeight(mapPoint);

                _trees.Add(new TreeDetail
                {
                    Id = node.Id,
                    Point = mapPoint
                });
            }
            return null;
        }

        #endregion
    }
}
