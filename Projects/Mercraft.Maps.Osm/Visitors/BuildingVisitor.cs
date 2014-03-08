using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Extensions;
using Mercraft.Core.Scene;

namespace Mercraft.Maps.Osm.Visitors
{
    public class BuildingVisitor: IElementVisitor
    {
        private readonly IScene _scene;

        public BuildingVisitor(IScene scene)
        {
            _scene = scene;
        }

        #region IElementVisitor implementation

        public void VisitNode(Node node)
        {
        }

        public void VisitWay(Way way)
        {
            if (!way.IsComplete || way.Nodes.Count < 3 || !IsBuilding(way.Tags)) 
                return;

            var building = new Building()
            {
                Id = way.Id.ToString(),
                Points = way.GetPoints(),
                Tags = way.Tags
                    .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                    .ToList()
            };

            // TODO Process tags and populate building object with any useful information 
            // for rendering (e.g. color, address, etc)
            _scene.AddBuilding(building);
        }

        public void VisitRelation(Relation relation)
        { 
        }

        #endregion

        private bool IsBuilding(ICollection<Tag> tags)
        {
            return tags.ContainsKey("building") && !tags.IsFalse("building");
        }

    }
}
