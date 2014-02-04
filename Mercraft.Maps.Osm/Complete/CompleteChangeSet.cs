
using System.Collections.Generic;
using Mercraft.Maps.Core;

namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Represents a changeset.
    /// </summary>
    public class CompleteChangeSet : CompleteOsmBase
    {
        /// <summary>
        /// Holds all the changes in this changeset.
        /// </summary>
        private IList<CompleteChange> _changes;

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <param name="id"></param>
        internal CompleteChangeSet(long id)
            :base(id)
        {
            _changes = new List<CompleteChange>();
        }

        #region Properties

        /// <summary>
        /// Returns an ordered list of all changes in this changeset.
        /// </summary>
        public IList<CompleteChange> Changes
        {
            get
            {
                return _changes;
            }
        }

        /// <summary>
        /// Returns the list of objects that this changeset applies to.
        /// </summary>
        public IList<CompleteOsmGeo> Objects
        {
            get
            {
                IList<CompleteOsmGeo> objs = new List<CompleteOsmGeo>();

                foreach (CompleteChange change in this.Changes)
                {
                    objs.Add(change.Object);
                }

                return objs;
            }
        }

        /// <summary>
        /// Returns the bounding box of this changeset.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get 
            {
                if (this.Objects.Count > 0)
                {
                    GeoCoordinateBox box = this.Objects[0].BoundingBox;

                    for (int idx = 1; idx < this.Objects.Count; idx++)
                    {
                        box = box + this.Objects[idx].BoundingBox;
                    }

                    return box;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the changeset type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.ChangeSet; }
        }

        #endregion
    }
}
