
namespace Mercraft.Maps.Core.Attributes
{
    /// <summary>
    /// Represents an index for storing attributes.
    /// </summary>
    public interface IGeometryAttributesIndex
    {
        /// <summary>
        /// Returns the attributes that belong to the given id.
        /// </summary>
        /// <param name="attributesId"></param>
        /// <returns></returns>
        GeometryAttributeCollection Get(uint attributesId);

        /// <summary>
        /// Adds new attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        uint Add(GeometryAttributeCollection attributes);
    }
}
