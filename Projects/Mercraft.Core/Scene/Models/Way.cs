using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    /// Represents a set of connected points. Used for roads
    /// </summary>
    public class Way: Model
    {
        public GeoCoordinate[] Points { get; set; }

        public override bool IsClosed
        {
            get { return Points[0] == Points[Points.Length - 1]; }
        }
    }
}
