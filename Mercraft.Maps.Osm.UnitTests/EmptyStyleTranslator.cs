using System;
using System.Collections.Generic;
using Mercraft.Maps.Core.Geometries;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Complete;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Interpreter;
using Mercraft.Maps.UI;
using Mercraft.Maps.UI.Scenes;

namespace Mercraft.Maps.Osm.UnitTests
{
    public class EmptyStyleTranslator: StyleTranslator
    {
        private GeometryInterpreter _geometryInterpreter = new SimpleGeometryInterpreter();
        private List<CompleteOsmGeo> _osmGeos = new List<CompleteOsmGeo>(); 
        public override void Translate(IScene scene, IProjection projection, CompleteOsmGeo osmGeo)
        {
            GeometryCollection collection = _geometryInterpreter.Interpret(osmGeo);
            foreach (Geometry geometry in collection)
            {
                if (geometry is LineairRing)
                { // a simple lineair ring.
                   // this.TranslateLineairRing(scene, projection, geometry as LineairRing);
                }
                else if (geometry is Polygon)
                { // a simple polygon.
                    //this.TranslatePolygon(scene, projection, geometry as Polygon);
                }
                else if (geometry is MultiPolygon)
                { // a multipolygon.
                    //this.TranslateMultiPolygon(scene, projection, geometry as MultiPolygon);
                }
            }

            _osmGeos.Add(osmGeo);
        }

        public override bool AppliesTo(Element element)
        {
            throw new NotImplementedException();
        }

        public IList<CompleteOsmGeo> TranslatedOsmGeos
        {
            get
            {
                return _osmGeos;
            }
        }
    }
}
