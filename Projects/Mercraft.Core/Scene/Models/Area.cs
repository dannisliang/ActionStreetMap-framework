using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    ///     Represents connected polygon. Used for buildings, parks
    /// </summary>
    public class Area : Model
    {
        public List<GeoCoordinate> Points { get; set; }

        public List<GeoCoordinate> Holes { get; set; }

        public override bool IsClosed
        {
            get
            {
                return
                    Points.Count > 2 &&
                    Points[0] == Points[Points.Count - 1];
            }
        }

        public override void Accept(IModelVisitor visitor)
        {
            visitor.VisitArea(this);
        }
    }
}