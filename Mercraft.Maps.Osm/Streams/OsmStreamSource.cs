
using System.Collections;
using System.Collections.Generic;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Streams
{
    /// <summary>
    /// Base class for any (streamable) source of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmStreamSource : IEnumerable<Element>, IEnumerator<Element>
    {
        /// <summary>
        /// Creates a new source.
        /// </summary>
        protected OsmStreamSource()
        {

        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();

        /// <summary>
        /// Returns the current item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract Element Current();

        /// <summary>
        /// Resets the source to the beginning.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanReset
        {
            get;
        }

        #region IEnumerator/IEnumerable Implementation

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Element> GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        Element IEnumerator<Element>.Current
        {
            get { return this.Current(); }
        }

        /// <summary>
        /// Disposes all resources associated with this source.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current(); }
        }

        #endregion
    }
}