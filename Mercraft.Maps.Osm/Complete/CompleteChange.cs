
namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Represents a change in a change set.
    /// </summary>
    public class CompleteChange
    {
        /// <summary>
        /// Contains the type of change.
        /// </summary>
        private ChangeType _type;

        /// <summary>
        /// Contains the object to change.
        /// </summary>
        private CompleteOsmGeo _obj;

        /// <summary>
        /// Creates a new change object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public CompleteChange(ChangeType type, CompleteOsmGeo obj)
        {
            _type = type;
            _obj = obj;
        }

        /// <summary>
        /// The type of this change.
        /// </summary>
        public ChangeType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// The object this change is for.
        /// </summary>
        public CompleteOsmGeo Object
        {
            get
            {
                return _obj;
            }
        }
    }

    /// <summary>
    /// Represents a type of change.
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// Create
        /// </summary>
        Create,
        /// <summary>
        /// Delete
        /// </summary>
        Delete,
        /// <summary>
        /// Modify
        /// </summary>
        Modify
    }
}
