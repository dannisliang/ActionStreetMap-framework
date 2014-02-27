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
        private IScene _scene;

        public BuildingVisitor(IScene scene)
        {
            _scene = scene;
        }

        public void VisitNode(Node node)
        {
        }

        public void VisitWay(Way way)
        {
            if (!IsBuilding(way.Tags)) 
                return;

            var building = new Building()
            {
                Points = way.GetPoints(),
                Tags = way.Tags.Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value)).ToList()
            };


            // Process tags and populate building object with information for rendering (e.g. color, address)

            _scene.AddBuilding(building);
        }

        public void VisitRelation(Relation relation)
        { 
        }


        private bool IsBuilding(ICollection<Tag> tags)
        {
            return tags.ContainsKey("building") && !tags.IsFalse("building");
        }
    }
}
