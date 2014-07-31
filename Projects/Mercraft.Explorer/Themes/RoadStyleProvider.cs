using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.World.Roads;
using Mercraft.Models.Roads;

namespace Mercraft.Explorer.Themes
{
    public class RoadStyleProvider : IRoadStyleProvider
    {
        private readonly Dictionary<string, List<RoadStyle>> _roadTypeStyleMapping;

        public RoadStyleProvider(Dictionary<string, List<RoadStyle>> roadTypeStyleMapping)
        {
            _roadTypeStyleMapping = roadTypeStyleMapping;
        }

        public RoadStyle Get(Road road)
        {
            // NOTE use first element's type
            //var type = road.Elements[0].Type;

            // TODO use smart logic to choose road style
            var type = _roadTypeStyleMapping.Keys.First();

            return _roadTypeStyleMapping[type][0];
        }
    }
}
