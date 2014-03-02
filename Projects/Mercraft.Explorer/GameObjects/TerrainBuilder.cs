using System;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class TerrainBuilder : ITerrainBuilder, IConfigurable
    {
        private readonly string LogTag = typeof (TerrainBuilder).Name;

        private string _materialPath;

        [Dependency]
        public ITrace Trace { get; set; }
        
        public GameObject Build(Tile tile)
        {
            var terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
            terrain.transform.position = new Vector3(tile.TileMapCenter.x, 0, tile.TileMapCenter.y);
            terrain.transform.localScale = new Vector3(tile.Size, 1, tile.Size);

            terrain.name = "tile";

            MeshHelper.TangentSolver(terrain.GetComponent<MeshFilter>().mesh);

            Trace.Info(LogTag, String.Format("Created floor on position: {0}, size: {1}", 
                terrain.transform.position, tile.Size));

            // add material
            var meshRenderer = terrain.GetComponent<MeshRenderer>();
            meshRenderer.renderer.material = Resources.Load<Material>(_materialPath);

            return terrain;
        }

        public void Configure(IConfigSection configSection)
        {
            _materialPath = configSection.GetString("material/@path");
        }
    }
}
