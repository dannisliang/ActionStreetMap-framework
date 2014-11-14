
using System.Collections.Generic;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Utilities;
using ActionStreetMap.Maps.Osm.Data;
using ActionStreetMap.Maps.Osm.Visitors;

namespace ActionStreetMap.Maps.Osm
{
    /// <summary>
    ///     Loads tile from OSM element source.
    /// </summary>
    public class OsmTileLoader: ITileLoader
    {
        private readonly IElementSourceProvider _elementSourceProvider;
        private readonly ElementManager _elementManager;
        private readonly IModelVisitor _modelVisitor;
        private readonly CompositeVisitor _compositeVisitor;

        /// <summary>
        ///     Creates OsmTileLoader.
        /// </summary>
        /// <param name="elementSourceProvider">Element source provider.</param>
        /// <param name="elementManager">Element manager.</param>
        /// <param name="modelVisitor">model visitor.</param>
        /// <param name="objectPool">Object pool.</param>
        [Dependency]
        public OsmTileLoader(IElementSourceProvider elementSourceProvider, ElementManager elementManager, 
            IModelVisitor modelVisitor, IObjectPool objectPool)
        {
            _elementSourceProvider = elementSourceProvider;
            _elementManager = elementManager;
            _modelVisitor = modelVisitor;

            _compositeVisitor = new CompositeVisitor(new List<IElementVisitor>
            {
                new WayVisitor(modelVisitor, objectPool),
                new NodeVisitor(modelVisitor, objectPool),
                new RelationVisitor(modelVisitor, objectPool)
            });
        }

        /// <inheritdoc />
        public void Load(Tile tile)
        {
            // get element source for given bounding box
            var elementSource = _elementSourceProvider.Get(tile.BoundingBox);

            // prepare tile
            tile.Accept(_modelVisitor);

            // get and visit areas, ways, relations
            _elementManager.VisitBoundingBox(tile.BoundingBox, elementSource, _compositeVisitor);

            // finalize by canvas visiting
            (new Canvas()).Accept(_modelVisitor);
        }
    }
}
