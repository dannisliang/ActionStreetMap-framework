using System.Collections.Generic;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections.LongIndex;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Models;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    /// Manages the objects in a scene for a style translator.
    /// </summary>
    public class SceneManager
    {
        /// <summary>
        /// Holds the scene.
        /// </summary>
        private IScene _scene;

        /// <summary>
        /// Holds the style translator.
        /// </summary>
        private ElementTranslator _translator;

        /// <summary>
        /// Holds the interpreted nodes.
        /// </summary>
        private LongIndex _translatedNodes;

        /// <summary>
        /// Holds the interpreted relations.
        /// </summary>
        private LongIndex _translatedRelations;

        /// <summary>
        /// Holds the interpreted way.
        /// </summary>
        private LongIndex _translatedWays;


        /// <summary>
        /// Creates a new style scene manager.
        /// </summary>
        public SceneManager(IScene scene, ElementTranslator translator)
        {
            _scene = scene;
            _translator = translator;

            _translatedNodes = new LongIndex();
            _translatedWays = new LongIndex();
            _translatedRelations = new LongIndex();

        }

        /// <summary>
        /// Fills the scene with objects from the given datasource that existing inside the given boundingbox with the given projection.
        /// </summary>
        public void FillScene(IDataSourceReadOnly dataSource, GeoCoordinateBox box, IProjection projection)
        {
            IList<Element> elements = dataSource.Get(box, null);
            foreach (var element in elements)
            { // translate each object into scene object.
                LongIndex index = null;

                element.Accept(new ElementVisitor(
                    _ => index = _translatedNodes,
                    _ => index = _translatedWays,
                    _ => index = _translatedRelations));
               
                if (!index.Contains(element.Id.Value))
                {
                    // object was not yet interpreted.
                    index.Add(element.Id.Value);

                    _translator.Translate(_scene, dataSource, projection, element);
                }
            }
        }
    }
}
