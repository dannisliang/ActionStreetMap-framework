
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    public sealed class TileFoundMessage
    {
        public Tile Tile { get; private set; }
        public MapPoint Position { get; private set; }

        public TileFoundMessage(Tile tile, MapPoint position)
        {
            Tile = tile;
            Position = position;
        }
    }

    public sealed class TileLoadStartMessage
    {
        public MapPoint TileCenter { get; private set; }

        public TileLoadStartMessage(MapPoint tileCenter)
        {
            TileCenter = tileCenter;
        }
    }

    public sealed class TileLoadFinishMessage
    {
        public Tile Tile { get; private set; }

        public TileLoadFinishMessage(Tile tile)
        {
            Tile = tile;
        }
    }
}
