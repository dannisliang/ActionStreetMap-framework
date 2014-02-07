using System;
using System.Collections.Generic;

namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Primive used as a base class for any osm object that has a meaning on the map (NodeIds, Ways and Relations).
    /// </summary>
    public abstract class Element
    {
        /// <summary>
        /// The id.
        /// </summary>
        public long? Id { get; set; }

        /*/// <summary>
        /// The type.
        /// </summary>
        public ElementType Type { get; protected set; }*/

        public abstract void Accept(IElementVisitor elementVisitor);

        /// <summary>
        /// The tags.
        /// </summary>
        public ICollection<Tag> Tags { get; set; }

        /// <summary>
        /// The changeset id.
        /// </summary>
        public long? ChangeSetId { get; set; }

        /// <summary>
        /// The visible flag.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// The timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// The version.
        /// </summary>
        public ulong? Version { get; set; }

        /// <summary>
        /// The userid.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string UserName { get; set; }
    }
}
