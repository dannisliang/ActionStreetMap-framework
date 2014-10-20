using Color32 = Mercraft.Core.Unity.Color32;
using Rect = Mercraft.Models.Geometry.Rect;

namespace Mercraft.Models.Roads
{
    /// <summary>
    ///     Defines road style.
    /// </summary>
    public class RoadStyle
    {
        /// <summary>
        ///     Path to Resource material
        /// </summary>
        public string Path { get; set; }

        // NOTE ignored so far by default RoadBuilder

        /// <summary>
        ///     Gets or sets height (TODO: do we need this?)    
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Gets or sets road width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///    Gets or sets material of road.
        /// </summary>
        public string Material { get; set; }

        /// <summary>
        ///     Gets or sets color.
        /// </summary>
        public Color32 Color { get; set; }

        /// <summary>
        ///     Gets or sets main uv map.
        /// </summary>
        public Rect MainUvMap { get; set; }

        /// <summary>
        ///     Gets or sets  turn uv map.
        /// </summary>
        public Rect TurnUvMap { get; set; }
    }
}
