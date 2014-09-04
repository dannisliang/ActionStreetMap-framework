namespace Mercraft.Core.Scene
{
    public enum SceneVisitResult
    {
        /// <summary>
        ///     None means that model wasn't processed
        /// </summary>
        None,

        /// <summary>
        ///     Partial means that model was processed partially for given scene
        ///     For example, non-flat road cannot be rendered fully if it crosses border 
        ///     between tiles (we cannot create height maps for other tiles in advance
        ///     due to performance reason)
        /// </summary>
        Partial,

        /// <summary>
        ///     Completed means that model was processed fully and should be processed again
        ///     (e.g. building, even for case when it is located on border of tiles - we render
        ///     it only once and ignore next time)
        /// </summary>
        Completed
    }
}
