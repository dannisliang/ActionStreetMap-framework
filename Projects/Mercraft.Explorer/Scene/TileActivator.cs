using System;
using Mercraft.Core.Scene;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Explorer.Scene
{
    public class TileActivator: ITileActivator
    {
        private const string Category = "tile";

        [Dependency]
        public ITrace Trace { get; set; }

        public void Activate(Tile tile)
        {
            // TODO
            Trace.Output(Category, String.Format("Tile activate: {0}", tile.MapCenter));
        }

        public void Deactivate(Tile tile)
        {
            // TODO
            Trace.Output(Category, String.Format("Tile deactivate: {0}", tile.MapCenter));
        }
    }
}
