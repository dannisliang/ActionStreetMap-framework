
using System;
using System.Collections.Generic;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Filters;
using Mercraft.Models;

namespace Mercraft.Maps.Osm.Data
{
    /// <summary>
    /// Base class for IDataSourceReadOnly-implementations.
    /// </summary>
    public abstract class DataSourceReadOnlyBase : IDataSourceReadOnly
    {
        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Node GetNode(long id)
        {
            IList<Node> nodes = this.GetNodes(new List<long>(new long[] { id }));
            if (nodes.Count > 0)
            {
                return nodes[0];
            }
            return null;
        }

        /// <summary>
        /// Returns all nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public abstract IList<Node> GetNodes(IList<long> ids);

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Relation GetRelation(long id)
        {
            IList<Relation> relations = this.GetRelations(new List<long>(new long[] { id }));
            if (relations.Count > 0)
            {
                return relations[0];
            }
            return null;
        }

        /// <summary>
        /// Returns the relation with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public abstract IList<Relation> GetRelations(IList<long> ids);

        /// <summary>
        /// Returns all relations containg the given object as a member.
        /// </summary>
        public abstract IList<Relation> GetRelationsFor(Element element);


        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Way GetWay(long id)
        {
            IList<Way> ways = this.GetWays(new List<long>(new long[] { id }));
            if (ways.Count > 0)
            {
                return ways[0];
            }
            return null;
        }

        /// <summary>
        /// Returns the ways with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public abstract IList<Way> GetWays(IList<long> ids);

        /// <summary>
        /// Returns all the ways containing the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IList<Way> GetWaysFor(long id);

        /// <summary>
        /// Returns all the ways containing the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual IList<Way> GetWaysFor(Node node)
        {
            return this.GetWaysFor(node.Id.Value);
        }

        /// <summary>
        /// Returns all objects inside the given boundingbox and according to the given filter.
        /// </summary>
        public abstract IList<Element> Get(BoundingBox bbox, IFilter filter);

      
    }
}
