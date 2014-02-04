
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Represents any source of nodes.
    /// </summary>
    public interface INodeSource
    {
        /// <summary>
        /// Returns a node with the given id from this source.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Node GetNode(long id);
    }
}
