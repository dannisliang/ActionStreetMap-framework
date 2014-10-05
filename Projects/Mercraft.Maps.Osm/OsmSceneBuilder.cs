
using System.Collections.Generic;
using Mercraft.Core.Scene.Models;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Core;
using Mercraft.Core.Scene;

namespace Mercraft.Maps.Osm
{
    public class OsmSceneBuilder: ISceneBuilder
    {
        private readonly IElementSourceProvider _elementSourceProvider;
        private readonly ElementManager _elementManager;
        private readonly IModelVisitor _modelVisitor;

        [Dependency]
        public OsmSceneBuilder(IElementSourceProvider elementSourceProvider, ElementManager elementManager, 
            IModelVisitor modelVisitor)
        {
            _elementSourceProvider = elementSourceProvider;
            _elementManager = elementManager;
            _modelVisitor = modelVisitor;
        }

        /// <summary>
        /// Builds scene using bounding box
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="bbox">Bounding box</param>
        public void Build(Tile tile, BoundingBox bbox)
        {
            var visitor = new CompositeVisitor(new List<IElementVisitor>
            {
                new WayVisitor(_modelVisitor),
                new NodeVisitor(_modelVisitor),
                new RelationVisitor(_modelVisitor)
            });

            var elementSource = _elementSourceProvider.Get(bbox);
            tile.Accept(_modelVisitor);
            _elementManager.VisitBoundingBox(bbox, elementSource, visitor);
            (new Canvas()).Accept(_modelVisitor);
        }
    }
}
