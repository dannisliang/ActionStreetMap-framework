
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections;
using Mercraft.Maps.Core.Geometries;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Interpreter;

namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Base class for all the osm data that represents an object on the map.
    /// 
    /// Nodes, Ways and Relations
    /// </summary>
    public abstract class CompleteOsmGeo : CompleteOsmBase
    {
        /// <summary>
        /// Creates a new Element object.
        /// </summary>
        /// <param name="id"></param>
        internal CompleteOsmGeo(long id)
            :base(id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Creates a new Element object with a string table.
        /// </summary>
        /// <param name="stringTable"></param>
        /// <param name="id"></param>
        internal CompleteOsmGeo(ObjectTable<string> stringTable, long id)
            : base(stringTable, id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Converts this Element object to an OsmGeoSimple object.
        /// </summary>
        /// <returns></returns>
        public abstract Element ToSimple();

        #region Geometry - Interpreter

        /// <summary>
        /// The interpreter for these objects.
        /// </summary>
        public static GeometryInterpreter GeometryInterperter = new SimpleGeometryInterpreter(); // set a default geometry interpreter.

        /// <summary>
        /// The geometries this OSM-object represents.
        /// </summary>
        private GeometryCollection _geometries;

        /// <summary>
        /// Returns the geometries this OSM-object represents.
        /// </summary>
        public GeometryCollection Geometries
        {
            get
            {
                if (_geometries == null)
                {
                    _geometries = CompleteOsmGeo.GeometryInterperter.Interpret(this);
                }
                return _geometries;
            }
        }

        /// <summary>
        /// Make sure the geometries of this objects will be recalculated.
        /// </summary>
        public void ResetGeometries()
        {
            _geometries = null;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// The bounding box of object.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get
            {
                return this.Geometries.Box;
            }
        }

        /// <summary>
        /// Gets/Sets the changeset id.
        /// </summary>
        public long? ChangeSetId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the visible flag.
        /// </summary>
        public bool Visible { get; set; }

        #endregion
    }
}
