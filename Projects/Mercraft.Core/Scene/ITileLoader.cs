using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    /// <summary>
    ///     Defines behavior of tile loader.
    /// </summary>
    public interface ITileLoader
    {
        /// <summary>
        ///     Loads given tile. This method triggers real loading and processing osm data.
        /// </summary>
        /// <param name="tile">Tile.</param>
        void Load(Tile tile);
    }
}
