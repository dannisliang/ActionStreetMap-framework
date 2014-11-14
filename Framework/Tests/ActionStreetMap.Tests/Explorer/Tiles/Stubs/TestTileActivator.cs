using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Explorer.Scene;

namespace ActionStreetMap.Tests.Explorer.Tiles.Stubs
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
