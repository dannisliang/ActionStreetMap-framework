using System.Collections.Generic;
using Mercraft.Core.World.Buildings;
using Mercraft.Core.World.Roads;

namespace Mercraft.Core.World
{
    /// <summary>
    ///     Holds all models in order to perform special processing on them depending on game needs
    /// </summary>
    public class WorldManager
    {
        private readonly HashSet<long>  _globalIds = new HashSet<long>();

        /// <summary>
        ///     Adds building 
        /// </summary>
        /// <param name="building">Building model</param>
        public void AddBuilding(Building building)
        {
            _globalIds.Add(building.Id);
        }

        /// <summary>
        ///     Adds road
        /// </summary>
        /// <param name="road">Road model</param>
        public void AddRoad(Road road)
        {
            road.Elements.ForEach(e => _globalIds.Add(e.Id));
        }

        /// <summary>
        ///     Adds general model Id
        /// </summary>
        /// <param name="id">Id of model</param>
        public void AddModel(long id)
        {
            _globalIds.Add(id);
        }

        /// <summary>
        ///     Checks whether given Id is present
        /// </summary>
        /// <param name="id">Id of model</param>
        /// <returns>True if model is present</returns>
        public bool Contains(long id)
        {
            return _globalIds.Contains(id);
        }
    }
}