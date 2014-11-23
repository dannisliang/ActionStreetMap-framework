using ActionStreetMap.Core;

namespace ActionStreetMap.Explorer
{
    /// <summary>
    ///     Represents entry point class for ASM logic.
    /// </summary>
    public interface IGameRunner
    {
        /// <summary>
        ///      Runs game with default geo coordinate as map center.
        /// </summary>
        void RunGame();

        /// <summary>
        ///      Runs game with provided coordinate as map center.
        /// </summary>
        /// <param name="startCoordinate">Geo coordinate for (0,0) map point.</param>
        void RunGame(GeoCoordinate startCoordinate);
    }
}