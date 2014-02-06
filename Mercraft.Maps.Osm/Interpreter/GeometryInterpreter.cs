
using System;
using Mercraft.Maps.Core.Geometries;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Interpreter
{
    /// <summary>
    /// Represents a geometry interpreter to convert OSM-objects to corresponding geometries.
    /// </summary>
    public abstract class GeometryInterpreter
    {
        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
        /// <returns></returns>
        public abstract GeometryCollection Interpret(Element element);
    }
}