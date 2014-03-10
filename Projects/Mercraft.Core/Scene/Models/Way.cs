using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    /// Represents a set of connected points. Used for roads
    /// </summary>
    public class Way: Model
    {
        public ICollection<GeoCoordinate> Points { get; set; }
    }
}
