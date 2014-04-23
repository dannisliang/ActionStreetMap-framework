using Mercraft.Core;

namespace Mercraft.Explorer
{
    /// <summary>
    /// Represents entry point class for Mercraft logic
    /// </summary>
    public interface IGameRunner
    {
        /// <summary>
        /// Runs game with provided coordinate as map center
        /// </summary>
        void RunGame(GeoCoordinate startCoordinate);
    }
}
