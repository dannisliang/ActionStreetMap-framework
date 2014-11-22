using System.Collections.Generic;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Utilities;
using ActionStreetMap.Osm.Entities;
using ActionStreetMap.Osm.Helpers;

namespace ActionStreetMap.Osm.Visitors
{
    /// <summary>
    ///     Relation visitor.
    /// </summary>
    public class RelationVisitor : ElementVisitor
    {
        /// <inheritdoc />
        public RelationVisitor(IModelVisitor modelVisitor, IObjectPool objectPool)
            : base(modelVisitor, objectPool)
        {
        }

        /// <inheritdoc />
        public override void VisitRelation(Relation relation)
        {
            var areas = new List<Area>(relation.Members.Count);
            // TODO use object pool
                //ObjectPool.NewList<Area>(relation.Members.Count);
            MultipolygonProcessor.FillAreas(relation, areas);
            foreach (var area in areas)
            {
                // TODO investigate this case
                if (area.Tags == null)
                    continue;
                ModelVisitor.VisitArea(area);
            }
        }
    }
}


