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
        /// <param name="dataSource"></param>
        /// <param name="box"></param>
        /// <param name="projection"></param>
        public void FillScene(IDataSourceReadOnly dataSource, GeoCoordinateBox box, IProjection projection)
        {
            IList<Element> elements = dataSource.Get(box, null);
            foreach (var element in elements)
            { // translate each object into scene object.
                LongIndex index = null;
                switch (element.Type)
                {
                    case ElementType.Node:
                        index = _translatedNodes;
                        break;
                    case ElementType.Way:
                        index = _translatedWays;
                        break;
                    case ElementType.Relation:
                        index = _translatedRelations;
                        break;
                }
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
