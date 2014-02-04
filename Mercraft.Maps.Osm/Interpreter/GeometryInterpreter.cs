
using System;
using Mercraft.Maps.Core.Collections.Tags;
using Mercraft.Maps.Core.Geometries;
using Mercraft.Maps.Osm.Complete;
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
        /// <param name="osmObject"></param>
        /// <returns></returns>
        public abstract GeometryCollection Interpret(CompleteOsmGeo osmObject);

        /// <summary>
        /// Returns true if the given tags collection contains potential area tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public abstract bool IsPotentiallyArea(TagsCollectionBase tags);

        /// <summary>
        /// Interprets an OSM-object and returns the correctponding geometry.
        /// </summary>
        /// <param name="simpleElement"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual GeometryCollection Interpret(Element simpleElement, IDataSourceReadOnly data)
        {
            switch (simpleElement.Type)
            {
                case ElementType.Node:
                    return this.Interpret(CompleteNode.CreateFrom(simpleElement as Node));
                case ElementType.Way:
                    return this.Interpret(CompleteWay.CreateFrom(simpleElement as Way, data));
                case ElementType.Relation:
                    return this.Interpret(CompleteRelation.CreateFrom(simpleElement as Relation, data));
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}