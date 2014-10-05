
using System.Collections.Generic;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core.Scene;

namespace Mercraft.Maps.Osm
{
    public class OsmTileLoader: ITileLoader
    {
        private readonly IElementSourceProvider _elementSourceProvider;
        private readonly ElementManager _elementManager;
        private readonly IModelVisitor _modelVisitor;
        private readonly IObjectPool _objectPool;

        [Dependency]
        public OsmTileLoader(IElementSourceProvider elementSourceProvider, ElementManager elementManager, 
            IModelVisitor modelVisitor, IObjectPool objectPool)
        {
            _elementSourceProvider = elementSourceProvider;
            _elementManager = elementManager;
            _modelVisitor = modelVisitor;
            _objectPool = objectPool;
        }

        /// <summary>
        ///     Loads tile
        /// </summary>
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
