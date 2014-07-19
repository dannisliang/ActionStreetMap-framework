namespace Mercraft.Core.World
{
    /// <summary>
    ///     Provides location information about the object
    /// </summary>
    public class Address
    {
        /// <summary>
        ///     Gets name, e.g. house number or road name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets street name
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        ///     Gets code, e.g. post code
        /// </summary>
        public string Code { get; set; }

        // TODO add other valuable information
    }
}