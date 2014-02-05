
using System;
using Mercraft.Maps.Core.Collections.Tags;
using Mercraft.Maps.Core.Geometries;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Interpreter
{
    /// <summary>
    /// Represents a geometry interpreter to convert OSM-objects to corresponding geometries.
    /// </summary>
    public abstract class GeometryInterpreter
    {
        /// <summary>
        /// Holds the default geometry interpreter.
        /// </summary>
        private static GeometryInterpreter _defaultInterpreter;

        /// <summary>
        /// Gets/sets the default interpreter.
        /// </summary>
        public static GeometryInterpreter DefaultInterpreter
        {
            get
            {
                if (_defaultInterpreter == null)
                { 
                    _defaultInterpreter = new SimpleGeometryInterpreter();
                }
                return _defaultInterpreter;
            }
            set
            {
                _defaultInterpreter = value;
            }
        }

        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
        /// <returns></returns>
        public abstract GeometryCollection Interpret(Element element);

        /// <summary>
        /// Returns true if the given tags collection contains potential area tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public abstract bool IsPotentiallyArea(TagsCollectionBase tags);
    }
}