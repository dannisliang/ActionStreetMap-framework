using System;
using System.Collections.Generic;
using Mercraft.Core.Scene.World.Buildings;
using Mercraft.Core.Scene.World.Infos;
using Mercraft.Core.Scene.World.Roads;

namespace Mercraft.Core.Scene
{
    /// <summary>
    ///    Provides the way to register and unregister world specific objects (e.g. buildings, roads, etc.) in tile.
    /// </summary>
    public class TileRegistry : IDisposable
    {
        // so far, we store only Ids
        private readonly HashSet<long> _localIds;

        // NOTE actually, this is workaround.
        // TODO should be designed better solution to prevent rendering of cross tile objects.
        /// <summary>
        ///     Contains global list of registered object ids
        /// </summary>
        private static readonly HashSet<long> GlobalIds = new HashSet<long>();

        /// <summary>
        ///     Creates ModelRegistry using global registered id hashset.
        /// </summary>
        internal TileRegistry()
        {
            _localIds = new HashSet<long>();
        }

        #region Registrations

        /// <summary>
        ///     Registers building ьщвуд.
        /// </summary>
        /// <param name="building">Building.</param>
        public void Register(Building building)
        {
            _localIds.Add(building.Id);
        }

        /// <summary>
        ///    Registres road. 
        /// </summary>
        /// <param name="road">Road.</param>
        public void Register(Road road)
        {
            road.Elements.ForEach(e => _localIds.Add(e.Id));
        }

        /// <summary>
        ///     Registers info.
        /// </summary>
        /// <param name="info">Info.</param>
        public void Register(Info info)
        {
            _localIds.Add(info.Id);
        }

        /// <summary>
        ///     Registers specific model id in global storage which prevents object with the same Id to be inserted in any tile.
        /// </summary>
        /// <param name="id">Id.</param>
        public void RegisterGlobal(long id)
        {
            GlobalIds.Add(id);
        }

        #endregion

        /// <summary>
        ///     Checks whether object with specific id is registered in global and local storages.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <returns>True if registration is found.</returns>
        public bool Contains(long id)
        {
            return GlobalIds.Contains(id) || _localIds.Contains(id);
        }

        #region IDisposable implementation

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // remove all registered ids from global list if they are in current registry
                foreach (var id in _localIds)
                {
                    if (GlobalIds.Contains(id))
                        GlobalIds.Remove(id);
                }

                _localIds.Clear();
            }
        }

        #endregion
    }
}
