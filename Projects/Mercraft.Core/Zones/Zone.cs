using System;
using System.Collections.Generic;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public class Zone
    {
        private readonly Tile _tile;
        private readonly IFloorBuilder _floorBuilder;
        private readonly IEnumerable<ISceneModelVisitor> _sceneModelVisitors;

        private GameObject _floor;

        public Zone(Tile tile, 
            IFloorBuilder floorBuilder,
            IEnumerable<ISceneModelVisitor> sceneModelVisitors)
        {
            _tile = tile;
            _floorBuilder = floorBuilder;
            _sceneModelVisitors = sceneModelVisitors;
        }

        public void Build()
        {
            _floor = _floorBuilder.Build(_tile);
            // Visit buildings
            foreach (var building in _tile.Scene.Buildings)
            {
                foreach (var sceneModelVisitor in _sceneModelVisitors)
                {
                    sceneModelVisitor.VisitBuilding(_tile.TileGeoCenter, _floor, building);
                }
            }
        }
    }
}
