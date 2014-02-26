using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Maps.Osm.Data;
using Mercraft.Maps.Osm.Visitors;
using Mercraft.Models;
using Mercraft.Models.Dependencies;
using Mercraft.Models.Scene;

namespace Mercraft.Maps.Osm
{
    public class OsmSceneBuilder: ISceneBuilder
    {
        private readonly IDataSourceProvider _dataSourceProvider;
        private readonly ElementManager _elementManager;

        [Dependency]
        public OsmSceneBuilder(IDataSourceProvider dataSourceProvider, ElementManager elementManager)
        {
            _dataSourceProvider = dataSourceProvider;
            _elementManager = elementManager;
        }

        /// <summary>
        /// Builds scene using center coordinate and bounding box
        /// TODO Probably, we needn't both parameters at the same time
        /// </summary>
        /// <param name="center">Center of bbox</param>
        /// <param name="bbox">Bounding box</param>
        public IScene Build(GeoCoordinate center, BoundingBox bbox)
        {
            var scene = new CountableScene();
            var visitor = new CompositeVisitor(new List<IElementVisitor>
            {
                new BuildingVisitor(scene)
            });

            var dataSource = _dataSourceProvider.Get(center);

            _elementManager.VisitBoundingBox(dataSource, bbox, visitor);

            return scene;
        }
    }
}
