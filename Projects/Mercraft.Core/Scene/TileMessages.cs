
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    #region Lookup
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
    #endregion

    #region Loading
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
    #endregion

    #region Activation
    public sealed class TileActivateMessage
    {
        public Tile Tile { get; private set; }

        public TileActivateMessage(Tile tile)
        {
            Tile = tile;
        }
    }

    public sealed class TileDeactivateMessage
    {
        public Tile Tile { get; private set; }

        public TileDeactivateMessage(Tile tile)
        {
            Tile = tile;
        }
    }

    public sealed class TileDestroyMessage
    {
        public Tile Tile { get; private set; }

        public TileDestroyMessage(Tile tile)
        {
            Tile = tile;
        }
    }

    #endregion
}
