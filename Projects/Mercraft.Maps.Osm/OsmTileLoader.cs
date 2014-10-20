
using System.Collections.Generic;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core.Scene;

namespace Mercraft.Maps.Osm
{
    /// <summary>
    ///     Loads tile from OSM element source.
    /// </summary>
    public class OsmTileLoader: ITileLoader
    {
        private readonly IElementSourceProvider _elementSourceProvider;
        private readonly ElementManager _elementManager;
        private readonly IModelVisitor _modelVisitor;
        private readonly IObjectPool _objectPool;

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
            _objectPool = objectPool;
        }

        /// <inheritdoc />
        public void Load(Tile tile)
        {
            var visitor = new CompositeVisitor(new List<IElementVisitor>
            {
                new WayVisitor(_modelVisitor, _objectPool),
                new NodeVisitor(_modelVisitor, _objectPool),
                new RelationVisitor(_modelVisitor, _objectPool)
            });

            var elementSource = _elementSourceProvider.Get(tile.BoundingBox);
            tile.Accept(_modelVisitor);
            _elementManager.VisitBoundingBox(tile.BoundingBox, elementSource, visitor);
            (new Canvas()).Accept(_modelVisitor);
        }
    }
}
