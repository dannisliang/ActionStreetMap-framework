
using System.Collections.Generic;

namespace Mercraft.Maps.Core.Geometries
{
    /// <summary>
    /// Represents a collection of geometry-objects.
    /// </summary>
    public class GeometryCollection : Geometry, IEnumerable<Geometry>
    {
        /// <summary>
        /// Holds the list of geometries.
        /// </summary>
        private List<Geometry> _geometries;

        /// <summary>
        /// Creates a new geometry collection.
        /// </summary>
        public GeometryCollection()
        {
            _geometries = new List<Geometry>();
        }

        /// <summary>
        /// Creates a new geometry collection.
        /// </summary>
        /// <param name="geometries"></param>
        public GeometryCollection(IEnumerable<Geometry> geometries)
        {
            _geometries = new List<Geometry>(geometries);
        }

        /// <summary>
        /// Returns the object count.
        /// </summary>
        public int Count
        {
            get
            {
                return _geometries.Count;
            }
        }

        /// <summary>
        /// Adds a new geometry.
        /// </summary>
        /// <param name="geometry"></param>
        public void Add(Geometry geometry)
        {
            _geometries.Add(geometry);
        }

        /// <summary>
        /// Adds all geometries in the given enumerable.
        /// </summary>
        /// <param name="geometries"></param>
        public void AddRange(IEnumerable<Geometry> geometries)
        {
            foreach (var geometry in geometries)
            {
                this.Add(geometry);
            }
        }

        /// <summary>
        /// Returns the geometry at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Geometry this[int idx]
        {
            get
            {
                return _geometries[idx];
            }
        }

        /// <summary>
        /// Returns the smallest bounding box containing all geometries in this collection.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get
            {
                GeoCoordinateBox box = null;
                foreach (Geometry geometry in _geometries)
                {
                    if (box == null)
                    {
                        box = geometry.Box;
                    }
                    else
                    {
                        box = box + geometry.Box;
                    }
                }
                return box;
            }
        }

        /// <summary>
        /// Returns true if at least one of the geometries in this collection exists inside the given boundingbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            foreach (Geometry geometry in _geometries)
            {
                if (geometry.IsInside(box))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all items from this collection.
        /// </summary>
        public void Clear()
        {
            _geometries.Clear();
        }

        #region IEnumerable<Geometry> Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the geometry collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Geometry> GetEnumerator()
        {
            return _geometries.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the geometry collection.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _geometries.GetEnumerator();
        }

        #endregion
    }
}
