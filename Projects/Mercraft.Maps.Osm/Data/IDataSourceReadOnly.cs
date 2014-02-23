
using System;
using System.Collections.Generic;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Filters;
using Mercraft.Models;


namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Represents a generic readonly data source. 
    /// </summary>
    public interface IDataSourceReadOnly : IElementSource
    {

        #region NodeIds

        /// <summary>
        /// Returns the node(s) with the given id(s).
        /// </summary>
        IList<Node> GetNodes(IList<long> ids);

        #endregion

        #region Relation

        /// <summary>
        /// Returns the relation(s) with the given id(s).
        /// </summary>
        IList<Relation> GetRelations(IList<long> ids);

        /// <summary>
        /// Returns all the relations for the given object.
        /// </summary>
        IList<Relation> GetRelationsFor(Element obj);

        #endregion

        #region Way

        /// <summary>
        /// Returns the way(s) with given id.
        /// </summary>
        IList<Way> GetWays(IList<long> ids);

        /// <summary>
        /// Returns the way(s) for a given node.
        /// </summary>
        IList<Way> GetWaysFor(long id);

        /// <summary>
        /// Returns the way(s) for a given node.
        /// </summary>
        IList<Way> GetWaysFor(Node node);

        #endregion

        #region Queries

        /// <summary>
        /// Returns all the objects in this dataset that evaluate the filter to true.
        /// </summary>
        IList<Element> Get(BoundingBox bbox, IFilter filter);

        #endregion

    }
}
