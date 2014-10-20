using Mercraft.Core;

namespace Mercraft.Models.Details
{
    /// <summary>
    ///     Represents a tree. Actually, it can define additional info like height, description, type, etc. as OSM supports this
    /// </summary>
    public class TreeDetail
    {
        /// <summary>
        ///     Tree id.
        /// </summary>
        public long Id;

        /// <summary>
        ///     Tree index.
        /// </summary>
        public int Index;

        /// <summary>
        ///     Gets or sets tree position
        /// </summary>
        public MapPoint Point { get; set; }
    }
}
