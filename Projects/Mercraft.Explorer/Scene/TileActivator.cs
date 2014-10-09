using System;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Explorer.Scene
{
    public class TileActivator: ITileActivator
    {
        private const string Category = "tile";

        [Dependency]
        public ITrace Trace { get; set; }

        public void Activate(Tile tile)
        {
            Trace.Output(Category, String.Format("Activate tile: {0}", tile.MapCenter));
            ProcessWithChildren(tile, true);
        }

        public void Deactivate(Tile tile)
        {
            Trace.Output(Category, String.Format("Deactivate tile: {0}", tile.MapCenter));
            ProcessWithChildren(tile, false);
        }

        public void Destroy(Tile tile)
        {
            Trace.Output(Category, String.Format("Destroy tile: {0}", tile.MapCenter));
            DestroyWithChildren(tile);
        }

        protected virtual void DestroyWithChildren(Tile tile)
        {
            UnityEngine.Object.Destroy(tile.GameObject.GetComponent<GameObject>());
        }

        protected virtual void ProcessWithChildren(Tile tile, bool active)
        {
            tile.GameObject.GetComponent<GameObject>().SetActive(active);
        }
    }
}
