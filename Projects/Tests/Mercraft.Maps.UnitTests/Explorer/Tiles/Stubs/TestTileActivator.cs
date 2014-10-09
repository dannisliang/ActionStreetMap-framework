using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Scene;

namespace Mercraft.Maps.UnitTests.Explorer.Tiles.Stubs
{
    public class TestTileActivator: TileActivator
    {
        protected override void DestroyWithChildren(Tile tile)
        {
            // Do nothing
        }

        protected override void ProcessWithChildren(Tile tile, bool active)
        {
            // Do nothing
        }
    }
}
