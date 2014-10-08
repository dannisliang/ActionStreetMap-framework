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
            Trace.Output(Category, String.Format("Tile activate: {0}", tile.MapCenter));
            ProcessWithChildren(tile, true);
        }

        public void Deactivate(Tile tile)
        {
            Trace.Output(Category, String.Format("Tile deactivate: {0}", tile.MapCenter));
            ProcessWithChildren(tile, false);
        }

        protected virtual void ProcessWithChildren(Tile tile, bool active)
        {
            tile.GameObject.GetComponent<GameObject>().SetActive(active);
        }
    }
}
