using System;
using System.Collections.Generic;
using ActionStreetMap.Core;
using ActionStreetMap.Maps.Osm.Entities;

namespace ActionStreetMap.Maps.Osm.Data
{
    /// <summary>
    ///     Represents an abstract source of Element objects.
    /// </summary>
    public interface IElementSource : IDisposable
    {
        /// <summary>
        ///     Returns elements which are located in the corresponding bbox.
        /// </summary>
        IEnumerable<Element> Get(BoundingBox bbox);

        /// <summary>
        ///     Returns a node with the given id from this source.
        /// </summary>
        Node GetNode(long id);

        /// <summary>
        ///     Returns a relation with the given id from this source.
        /// </summary>
        Relation GetRelation(long id);

        /// <summary>
        ///     Returns a way with the given id from this source.
        /// </summary>
        Way GetWay(long id);

        /// <summary>
        ///     Resets internal state.
        /// </summary>
        void Reset();
    }
}