
using System;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections;
using Mercraft.Maps.Core.Collections.Tags;

namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Base class of all osm objects.
    /// 
    /// ChangeSets, Nodes, Ways and Relations.
    /// </summary>   
    public abstract class CompleteOsmBase : IEquatable<CompleteOsmBase>
    {
        /// <summary>
        /// Holds the id of the object.
        /// </summary>
        private long _id;

        /// <summary>
        /// Creates a new base object.
        /// </summary>
        /// <param name="id"></param>
        internal CompleteOsmBase(long id)
            : this(null, id)
        {

        }

        /// <summary>
        /// Creates a new base object with a string table for the tags.
        /// </summary>
        /// <param name="stringTable"></param>
        /// <param name="id"></param>
        internal CompleteOsmBase(ObjectTable<string> stringTable, long id)
        {
            _id = id;

            if (stringTable != null)
            {
                this.Tags = new StringTableTagsCollection(stringTable);
            }
            else
            {
                this.Tags = new TagsCollection();
            }
        }

        /// <summary>
        /// The user that created this object
        /// </summary>
        public string User
        {
            get;
            set;
        }
        /// <summary>
        /// The user id.
        /// </summary>
        public long? UserId 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// The id of this object.
        /// </summary>
        public long Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Returns the bounding box for this object.
        /// </summary>
        public abstract GeoCoordinateBox BoundingBox
        {
            get;
        }

        /// <summary>
        /// Returns the type of osm data.
        /// </summary>
        public abstract CompleteOsmType Type
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the version.
        /// </summary>
        public long? Version
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the tags dictionary.
        /// </summary>
        public TagsCollectionBase Tags { get; protected set; }

        /// <summary>
        /// Returns true if a and b represent the same object.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(CompleteOsmBase a, CompleteOsmBase b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        /// <summary>
        /// Returns true if a and b do not represent the same object.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(CompleteOsmBase a, CompleteOsmBase b)
        {
            return !(a == b);
        }

        #region IEquatable<OsmBase> Members

        /// <summary>
        /// Returns true if the given object equals this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CompleteOsmBase other)
        {
            if (other == null)
            {
                return false;
            }
            if (other._id == this.Id
                && other.Type == this.Type)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a hascode for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Type.GetHashCode() 
                ^ this.Id.GetHashCode();
        }

        /// <summary>
        /// Returns true if object equals the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is CompleteOsmBase)
            {
                return this.Equals(obj as CompleteOsmBase);
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}",
                this.Type.ToString(),
                this.Id);
        }
    }
}
