
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Data
{
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
}
