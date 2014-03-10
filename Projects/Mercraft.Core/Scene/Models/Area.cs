using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    /// Represents connected polygon. Used for buildings, parks
    /// </summary>
    public class Area: Model
    {
        public ICollection<GeoCoordinate> Points { get; set; }
    }
}
