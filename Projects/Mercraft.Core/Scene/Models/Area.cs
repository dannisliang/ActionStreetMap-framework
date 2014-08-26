namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    ///     Represents connected polygon. Used for buildings, parks
    /// </summary>
    public class Area : Model
    {
        public GeoCoordinate[] Points { get; set; }

        public GeoCoordinate[] Holes { get; set; }

        public override bool IsClosed
        {
            get
            {
                return
                    Points.Length > 2 &&
                    Points[0] == Points[Points.Length - 1];
            }
        }
    }
}