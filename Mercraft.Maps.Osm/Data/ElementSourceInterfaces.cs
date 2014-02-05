
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Represents an abstract source of Element objects.
    /// </summary>
    public interface IElementSource : INodeSource, IWaySource, IRelationSource
    {

    }

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

    /// <summary>
    /// Represents any source of relations.
    /// </summary>
    public interface IRelationSource
    {
        /// <summary>
        /// Returns a relation with the given id from this source.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Relation GetRelation(long id);
    }

    /// <summary>
    /// Represents any source of ways.
    /// </summary>
    public interface IWaySource
    {
        /// <summary>
        /// Returns a way with the given id from this source.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Way GetWay(long id);
    }
}