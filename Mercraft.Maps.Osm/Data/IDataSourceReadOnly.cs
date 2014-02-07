
using System;
using System.Collections.Generic;
using Mercraft.Maps.Core;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Filters;


namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Represents a generic readonly data source. 
    /// </summary>
    public interface IDataSourceReadOnly : IElementSource
    {
        /// <summary>
        /// Returns the bounding box of the data in this source if possible.
        /// </summary>
        GeoCoordinateBox BoundingBox { get; }

        /// <summary>
        /// The unique id for this datasource.
        /// </summary>
        Guid Id { get; }
        
        #region Features

        /// <summary>
        /// Returns true if this datasource is bounded.
        /// </summary>
        bool HasBoundinBox { get; }

        /// <summary>
        /// Returns true if this datasource is readonly.
        /// </summary>
        bool IsReadOnly { get; }

        #endregion

        #region NodeIds

        /// <summary>
        /// Returns the node(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<Node> GetNodes(IList<long> ids);

        #endregion

        #region Relation

        /// <summary>
        /// Returns the relation(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<Relation> GetRelations(IList<long> ids);

        /// <summary>
        /// Returns all the relations for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IList<Relation> GetRelationsFor(Element obj);

        #endregion

        #region Way

        /// <summary>
        /// Returns the way(s) with given id.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<Way> GetWays(IList<long> ids);

        /// <summary>
        /// Returns the way(s) for a given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<Way> GetWaysFor(long id);

        /// <summary>
        /// Returns the way(s) for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IList<Way> GetWaysFor(Node node);

        #endregion

        #region Queries

        /// <summary>
        /// Returns all the objects in this dataset that evaluate the filter to true.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IList<Element> Get(GeoCoordinateBox box, IFilter filter);

        #endregion

    }
}
