
using System;
using System.Collections.Generic;
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
        public abstract IEnumerable<Geometry> Interpret(Element element);
    }
}