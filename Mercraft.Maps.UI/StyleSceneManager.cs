using System.Collections.Generic;
using Mercraft.Maps.Core;
using Mercraft.Maps.Core.Collections.LongIndex;
using Mercraft.Maps.Core.Projections;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.UI.Scenes;

namespace Mercraft.Maps.UI
{
    /// <summary>
    /// Manages the objects in a scene for a style translator.
    /// </summary>
    public class StyleSceneManager
    {
        /// <summary>
        /// Holds the scene.
        /// </summary>
        private IScene _scene;

        /// <summary>
        /// Holds the style translator.
        /// </summary>
        private StyleTranslator _translator;

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
        public StyleSceneManager(IScene scene, StyleTranslator translator)
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
            IList<Element> osmGeos = dataSource.Get(box, null);
            foreach (var osmGeo in osmGeos)
            { // translate each object into scene object.
                LongIndex index = null;
                switch (osmGeo.Type)
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
                if (!index.Contains(osmGeo.Id.Value))
                {
                    // object was not yet interpreted.
                    index.Add(osmGeo.Id.Value);

                    _translator.Translate(_scene, dataSource, projection, osmGeo);
                }
            }
        }
    }
}
