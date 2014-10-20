using System;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Explorer.Scene
{
    /// <summary>
    ///     Represents default tile activator. 
    /// </summary>
    public class TileActivator: ITileActivator
    {
        private const string Category = "tile";

        /// <summary>
        ///     Trace.
        /// </summary>
        [Dependency]
        public ITrace Trace { get; set; }

        /// <inheritdoc />
        public void Activate(Tile tile)
        {
            Trace.Output(Category, String.Format("Activate tile: {0}", tile.MapCenter));
            ProcessWithChildren(tile, true);
        }

        /// <inheritdoc />
        public void Deactivate(Tile tile)
        {
            Trace.Output(Category, String.Format("Deactivate tile: {0}", tile.MapCenter));
            ProcessWithChildren(tile, false);
        }

        /// <inheritdoc />
        public void Destroy(Tile tile)
        {
            Trace.Output(Category, String.Format("Destroy tile: {0}", tile.MapCenter));
            DestroyWithChildren(tile);
        }

        /// <summary>
        ///     Destroys tile using UnityEngine.Object.Destroy.
        /// </summary>
        /// <param name="tile">Tile.</param>
        protected virtual void DestroyWithChildren(Tile tile)
        {
            UnityEngine.Object.Destroy(tile.GameObject.GetComponent<GameObject>());
        }

        /// <summary>
        ///     Calls UnityEngine.GameObject.SetActive(active) for given tile.
        /// </summary>
        /// <param name="tile">Tile.</param>
        /// <param name="active">Active flag.</param>
        protected virtual void ProcessWithChildren(Tile tile, bool active)
        {
            tile.GameObject.GetComponent<GameObject>().SetActive(active);
        }
    }
}
