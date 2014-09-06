using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.World.Roads;
using Mercraft.Explorer.Helpers;
using Mercraft.Maps.Osm.Helpers;
using Mercraft.Models.Terrain;

namespace Mercraft.Explorer.Scene
{
    public class TerrainModelBuilder: IModelBuilder
    {
        public string Name { get { return "terrain"; } }

        private readonly List<AreaSettings> _areas;
        private readonly List<AreaSettings> _elevations;
        private readonly List<RoadElement> _roadElements;

        public TerrainModelBuilder(List<AreaSettings> areas, List<AreaSettings> elevations, List<RoadElement> roadElements)
        {
            _areas = areas;
            _elevations = elevations;
            _roadElements = roadElements;
        }

        public IGameObject BuildArea(Tile tile, Rule rule, Area area)
        {
            if (rule.IsTerrain())
            {
                _areas.Add(new AreaSettings()
                {
                    ZIndex = rule.GetZIndex(),
                    SplatIndex = rule.GetSplatIndex(),
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
                    Width = (int)Math.Round(rule.GetWidth() / 2),
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
    }
}
