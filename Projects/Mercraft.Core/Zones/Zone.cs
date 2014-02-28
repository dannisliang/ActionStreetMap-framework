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
        private readonly IEnumerable<ISceneModelVisitor> _sceneModelVisitors;

        private GameObject _floor;

        public Zone(Tile tile, IEnumerable<ISceneModelVisitor> sceneModelVisitors)
        {
            _tile = tile;
            _sceneModelVisitors = sceneModelVisitors;
        }

        public void Build()
        {
            CreateFloor();
            CreateGameObjects();
        }

        private void CreateFloor()
        {
            // TODO extract this as separate behavior
            _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _floor.transform.position = new Vector3(500, 30, 500);
            _floor.transform.localScale = new Vector3(10, 1, 10);
        }

        private void CreateGameObjects()
        {
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
