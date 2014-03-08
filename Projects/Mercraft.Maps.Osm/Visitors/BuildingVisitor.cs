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
        private const int DefaultBuildingLevelsCount = 5;

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

                // TODO remove tags on building
                Tags = way.Tags
                    .Select(tag => new KeyValuePair<string, string>(tag.Key, tag.Value))
                    .ToList()
            };

            ProcessTags(building, way);

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

        private void ProcessTags(Building building, Way way)
        {
            building.LevelCount = GetTagValue("building:levels", way, DefaultBuildingLevelsCount, ConverterProvider.IntConverter);
        }

        private T GetTagValue<T>(string tagKey, Way way, T defaultValue, Func<string, T> converter)
        {
            var @value = "";
            if (way.Tags.TryGetValue(tagKey, out @value))
            {
                var convertedValue = converter(@value);
                if (!EqualityComparer<T>.Default.Equals(convertedValue, default(T)))
                    return convertedValue;
            }
            return defaultValue;
        }
    }
}
