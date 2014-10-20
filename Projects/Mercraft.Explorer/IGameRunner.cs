using Mercraft.Core;

namespace Mercraft.Explorer
{
    /// <summary>
    ///     Represents entry point class for Mercraft logic.
    /// </summary>
    public interface IGameRunner
    {
        /// <summary>
        ///      Runs game with provided coordinate as map center.
        /// </summary>
        /// <param name="startCoordinate">Geo coordinate for (0,0) map point.</param>
        void RunGame(GeoCoordinate startCoordinate);
    }
}