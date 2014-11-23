
using System.Collections.Generic;

namespace ActionStreetMap.Infrastructure.Utilities
{
    /// <summary>
    ///     Represents object pool which is used to reduced amount of memory allocations.
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        ///     Returns object from pool or create new one.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <returns>Object.</returns>
        T New<T>();

        /// <summary>
        ///     Stores object in pool.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="obj">Object.</param>
        void Store<T>(T obj);

        /// <summary>
        ///     Returns list from pool or creates new one.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <returns>List.</returns>
        List<T> NewList<T>();

        /// <summary>
        ///     Returns list from pool or creates new one.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="capacity">Desired list campacity.</param>
        /// <returns></returns>
        List<T> NewList<T>(int capacity);

        /// <summary>
        /// Stores list in pool.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="list">List.</param>
        void Store<T>(List<T> list);

        /// <summary>
        ///     Reduces internal buffers.
        /// </summary>
        void Shrink();
    }
}
