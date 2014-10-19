namespace Mercraft.Core.World.Infos
{
    /// <summary>
    ///     This class is associated with OSM node 
    /// </summary>
    public class Info
    {
        /// <summary>
        ///     Gets or sets style key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets geocoordinate
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }
    }
}
