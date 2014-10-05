
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Tiles
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

    public sealed class TileBuildStartMessage
    {
        public MapPoint TileCenter { get; private set; }

        public TileBuildStartMessage(MapPoint tileCenter)
        {
            TileCenter = tileCenter;
        }
    }

    public sealed class TileBuildFinishMessage
    {
        public Tile Tile { get; private set; }

        public TileBuildFinishMessage(Tile tile)
        {
            Tile = tile;
        }
    }
}
