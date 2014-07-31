using System.Collections.Generic;
using Mercraft.Core.Unity;

namespace Mercraft.Core.World.Roads
{
    /// <summary>
    ///     Represents road which can consist of different road elements
    ///     This is useful for smooth road rendering
    /// </summary>
    public class Road
    {
        public IGameObject GameObject { get; set; }

        public List<RoadElement> Elements { get; set; }
    }
}