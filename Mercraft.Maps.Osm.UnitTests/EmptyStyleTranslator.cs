using System;
using System.Collections.Generic;
using Mercraft.Maps.Core.Geometries;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Interpreter;
using Mercraft.Maps.UI;
using Mercraft.Maps.UI.Scenes;

namespace Mercraft.Maps.Osm.UnitTests
{
    public class EmptyStyleTranslator: StyleTranslator
    {
        private List<Element> _osmGeos = new List<Element>();


        public override void Translate(IScene scene, IProjection projection, Node node)
        {
            _osmGeos.Add(node);
        }

        public override void Translate(IScene scene, IProjection projection, Way way)
        {
            _osmGeos.Add(way);
        }

        public override void Translate(IScene scene, IProjection projection, Relation relation)
        {
            _osmGeos.Add(relation);
        }

        public override bool AppliesTo(Element element)
        {
            throw new NotImplementedException();
        }

        public IList<Element> TranslatedOsmGeos
        {
            get
            {
                return _osmGeos;
            }
        }
    }
}
