namespace Mercraft.Core.World.Roads
{
    /// <summary>
    ///     Represents certain part of road
    /// </summary>
    public class RoadElement
    {
        public long Id { get; set; }

        public Address Address { get; set; }

        public int Lanes { get; set; }

        public float Width { get; set; }

        /// <summary>
        ///     True if this road element isn't connected with previous one
        /// </summary>
        public bool IsNotContinuation { get; set; }

        /// <summary>
        ///     Gets or sets actual type of road element. Useful for choosing of road material
        /// </summary>
        public string Type { get; set; }

        public MapPoint[] Points { get; set; }

        public float ZIndex { get; set; }
    }
}