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
        private readonly ITerrainBuilder _terrainBuilder;
        private readonly IEnumerable<ISceneModelVisitor> _sceneModelVisitors;

        private GameObject _floor;

        public Zone(Tile tile, 
            ITerrainBuilder terrainBuilder,
            IEnumerable<ISceneModelVisitor> sceneModelVisitors)
        {
            _tile = tile;
            _terrainBuilder = terrainBuilder;
            _sceneModelVisitors = sceneModelVisitors;
        }

        public void Build()
        {
            _floor = _terrainBuilder.Build(_tile);
            // Visit buildings
            foreach (var building in _tile.Scene.Buildings)
            {
                foreach (var sceneModelVisitor in _sceneModelVisitors)
                {
                    // TODO probably, we need to return built game object 
                    // to be able to perform cleanup on our side
                    sceneModelVisitor.VisitBuilding(_tile.RelativeNullPoint, _floor, building);
                }
            }
        }
    }
}
