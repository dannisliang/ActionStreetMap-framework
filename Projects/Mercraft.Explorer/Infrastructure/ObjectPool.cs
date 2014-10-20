using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Infrastructure.Utilities;

namespace Mercraft.Explorer.Infrastructure
{
    /// <summary>
    ///     Defines default object pool.
    /// </summary>
    public class ObjectPool: IObjectPool
    {
        private readonly ObjectListPool<MapPoint>  _mapPointListPool = new ObjectListPool<MapPoint>(64, 32);
        private readonly ObjectListPool<GeoCoordinate> _geoCoordListPool = new ObjectListPool<GeoCoordinate>(64, 32);

        /// <inheritdoc />
        public T New<T>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Store<T>(T obj)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public List<T> NewList<T>()
        {
            if (typeof (T) == typeof (MapPoint))
                return _mapPointListPool.New() as List<T>;

            if (typeof (T) == typeof (GeoCoordinate))
                return _geoCoordListPool.New() as List<T>;
            
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public List<T> NewList<T>(int capacity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Store<T>(List<T> list)
        {
            if (typeof(T) == typeof(MapPoint))
                _mapPointListPool.Store(list as List<MapPoint>);
            else if (typeof(T) == typeof(GeoCoordinate))
                _geoCoordListPool.Store(list as List<GeoCoordinate>);
            else
                throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Shrink()
        {
            // TODO reduce amount of stored data
        }
    }
}
