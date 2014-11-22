using System;
using System.Collections.Generic;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;

namespace ActionStreetMap.Explorer.Scene
{
    public class RelationBuilder
    {
        private const string OutlineTypeKey = "outline";
        private HashSet<long> _outlines = new HashSet<long>(); 
        private List<Relation> _relations = new List<Relation>(32);

        [Dependency]
        public ITrace Trace { get; set; }

        public void Add(Relation relation)
        {
            _relations.Add(relation);
            if (relation.RoleMap == null)
                return;

            if (relation.RoleMap.ContainsKey(OutlineTypeKey))
            {
                foreach (long id in relation.RoleMap[OutlineTypeKey])
                    _outlines.Add(id);
            }
            // TODO process parts
        }

        public void Build(IModelVisitor visitor)
        {
            foreach (var relation in _relations)
            {
                if (relation.Areas==null)
                    continue;
                
                foreach (var area in relation.Areas)
                {
                    // filter outline
                    if(!_outlines.Contains(area.Id))
                        visitor.VisitArea(area);
                    else
                        Trace.Output("Experimental RelationBuilder:", String.Format("filtered out: {0}", area));
                }
            }

            _outlines.Clear();
            _relations.Clear();
        }
    }
}
