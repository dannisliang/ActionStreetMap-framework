using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.MapCss;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Explorer.Themes;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Details;
using Mercraft.Models.Roads;
using Mercraft.Models.Terrain;
using UnityEngine;

namespace Mercraft.Explorer.Scene
{
    public class TileModelLoader : ITileLoader, IModelVisitor, IModelBuilder
    {
        private readonly IHeightMapProvider _heighMapProvider;
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IRoadBuilder _roadBuilder;
        private readonly IEnumerable<IModelBuilder> _builders;
        private readonly IEnumerable<IModelBehaviour> _behaviours;
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
            IEnumerable<IModelBuilder> builders, IEnumerable<IModelBehaviour> behaviours)
        {
            _heighMapProvider = heighMapProvider;
            _terrainBuilder = terrainBuilder;
            _roadBuilder = roadBuilder;
            _builders = builders.ToArray();
            _behaviours = behaviours.ToArray();
            _gameObjectFactory = gameObjectFactory;
            _themeProvider = themeProvider;
            _stylesheet = stylesheetProvider.Get();
        }

        #region ITileLoader

        public void Load(Tile tile)
        {
            _tile = tile;

            var heightMapResolution = _stylesheet.GetRule(tile.Scene.Canvas, false).GetHeightMapSize();
            tile.GameObject = _gameObjectFactory.CreateNew("tile");
            tile.HeightMap = _heighMapProvider.GetHeightMap(tile, heightMapResolution);

            foreach (var area in tile.Scene.Areas)
                area.Accept(this);
            foreach (var way in tile.Scene.Ways)
                way.Accept(this);
            foreach (var node in tile.Scene.Nodes)
                node.Accept(this);
            tile.Scene.Canvas.Accept(this);

            // clear collections to reuse
            _areas.Clear();
            _elevations.Clear();
            _roadElements.Clear();
            _trees.Clear();
        }

        #endregion

        #region IModelVisitor

        public void VisitArea(Area area)
        {
            var rule = _stylesheet.GetRule(area);
            if (ShouldUseBuilder(rule, area))
            {
                // NOTE this is work-around as we cannot register instance in our container multiply time
                // with default RegisterComponent
                BuildArea(_tile, rule, area);

                var modelBuilder = rule.GetModelBuilder(_builders);
                if (modelBuilder != null)
                {
                    var gameObject = modelBuilder.BuildArea(_tile, rule, area);
                    AttachExtras(gameObject, rule, area);
                }
            }
        }

        public void VisitWay(Way way)
        {
            var rule = _stylesheet.GetRule(way);
            if (ShouldUseBuilder(rule, way))
            {
                // NOTE this is work-around as we cannot register instance in our container multiply time
                // with default RegisterComponent
                BuildWay(_tile, rule, way);

                var modelBuilder = rule.GetModelBuilder(_builders);
                if (modelBuilder != null)
                {
                    var gameObject = modelBuilder.BuildWay(_tile, rule, way);
                    AttachExtras(gameObject, rule, way);
                }
            }
        }

        public void VisitNode(Node node)
        {
            var rule = _stylesheet.GetRule(node);
            if (ShouldUseBuilder(rule, node))
            {
                BuildNode(_tile, rule, node);
            }
        }

        public void VisitCanvas(Canvas canvas)
        {
            var rule = _stylesheet.GetRule(_tile.Scene.Canvas, false);

            // TODO this should be done by road composer
            var roads = _roadElements.Select(re => new Road()
            {
                Elements = new List<RoadElement>() {re},
                GameObject =
                    _gameObjectFactory.CreateNew(String.Format("road [{0}] {1}/ ", re.Id, re.Address), _tile.GameObject),
            }).ToArray();

            if (_tile.HeightMap.IsFlat)
                _tile.HeightMap.MaxElevation = rule.GetHeight();

            _terrainBuilder.Build(_tile.GameObject, new TerrainSettings()
            {
                Tile = _tile,
                AlphaMapSize = rule.GetAlphaMapSize(),
                CenterPosition = new Vector2(_tile.MapCenter.X, _tile.MapCenter.Y),
                CornerPosition = new Vector2(_tile.BottomLeft.X, _tile.BottomLeft.Y),
                PixelMapError = rule.GetPixelMapError(),
                ZIndex = rule.GetZIndex(),
                TextureParams = rule.GetTextureParams(),
                // TODO define in config and parse
                DetailParams = new List<List<string>>()
                {
                    new List<string>(),
                },
                Areas = _areas,
                Elevations = _elevations,
                Trees = _trees,
                Roads = roads,
                RoadBuilder = _roadBuilder,
                RoadStyleProvider = _themeProvider.Get().GetStyleProvider<IRoadStyleProvider>()
            });
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
                //var behaviour = rule.GetModelBehaviour(_behaviours);
                //if (behaviour != null)
                //    behaviour.Apply(gameObject, model);
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
                _areas.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    SplatIndex = rule.GetSplatIndex(),
                    DetailIndex = 0,
                    Points = PolygonHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points)
                });
            }

            if (rule.IsElevation())
            {
                _elevations.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    Points = PolygonHelper.GetVerticies2D(tile.RelativeNullPoint, area.Points)
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
                _roadElements.Add(new RoadElement()
                {
                    Id = way.Id,
                    Address = AddressExtractor.Extract(way.Tags),
                    Width = (int) Math.Round(rule.GetWidth()/2),
                    ZIndex = rule.GetZIndex(),
                    Points = way.Points.Select(p =>
                    {
                        var mapPoint = GeoProjection.ToMapCoordinate(tile.RelativeNullPoint, p);
                        mapPoint.Elevation = tile.HeightMap.LookupHeight(mapPoint);
                        return mapPoint;
                    }).ToArray()
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

                _trees.Add(new TreeDetail()
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
