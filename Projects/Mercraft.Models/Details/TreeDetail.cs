using Mercraft.Core;

namespace Mercraft.Models.Details
{
    /// <summary>
    ///     Represents a tree. Actually, it can define additional info like
    ///     height, description, type, etc. as OSM supports this
    /// </summary>
    public class TreeDetail
    {
        public long Id { get; set; }

        public int Index { get; set; }
        public MapPoint Point { get; set; }
    }
}
