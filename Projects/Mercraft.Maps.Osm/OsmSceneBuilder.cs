using System.Collections.Generic;

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

        [Dependency]
        public OsmSceneBuilder(IElementSourceProvider elementSourceProvider, ElementManager elementManager)
        {
            _elementSourceProvider = elementSourceProvider;
            _elementManager = elementManager;
        }

        /// <summary>
        /// Builds scene using center coordinate and bounding box
        /// TODO Probably, we needn't both parameters at the same time
        /// </summary>
        /// <param name="bbox">Bounding box</param>
        public IScene Build(BoundingBox bbox)
        {
            var scene = new MapScene();
            var visitor = new CompositeVisitor(new List<IElementVisitor>
            {
                new AreaVisitor(scene)
            });

            var elementSource = _elementSourceProvider.Get(bbox);

            _elementManager.VisitBoundingBox(bbox, elementSource, visitor);

            return scene;
        }
    }
}
