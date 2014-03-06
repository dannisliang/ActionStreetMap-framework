using System;
using System.Collections.Generic;
using Mercraft.Maps.Osm.Visitors;

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

        /// <summary>
        /// The tags.
        /// </summary>
        public ICollection<Tag> Tags { get; set; }

        /// <summary>
        /// The visible flag.
        /// </summary>
        public bool? Visible { get; set; }

        public abstract void Accept(IElementVisitor elementVisitor);
    }
}
