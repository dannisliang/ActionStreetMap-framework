using System.Collections.Generic;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Utilities;
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
        public override void VisitRelation(Entities.Relation relation)
        {
            string actualValue;
            var modelRelation = new Relation()
            {
                Id = relation.Id,
                Tags = relation.Tags,
            };

            if (relation.Tags != null && relation.Tags.TryGetValue("type", out actualValue) &&
                actualValue == "multipolygon")
            {
                // TODO use object pool
                modelRelation.Areas = new List<Area>(relation.Members.Count);
                MultipolygonProcessor.FillAreas(relation, modelRelation.Areas);
            }
            else
            {
                // NOTE we have to remember relation memebers and their roles
                // this is necessary to detect:
                // 1. building parts
                // 2. outline areas which have building-specific tags but shouldn't be rendered
                // ...
                var roleMap = new Dictionary<string, HashSet<long>>(relation.Members.Count);
                foreach (var relationMember in relation.Members)
                {
                    if (!roleMap.ContainsKey(relationMember.Role))
                        roleMap.Add(relationMember.Role, new HashSet<long>());
                    roleMap[relationMember.Role].Add(relationMember.MemberId);
                }

                modelRelation.RoleMap = roleMap;
            }     
    
            ModelVisitor.VisitRelation(modelRelation);
        }
    }
}


