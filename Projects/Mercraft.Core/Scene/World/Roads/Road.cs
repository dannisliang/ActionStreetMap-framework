using System.Collections.Generic;
using Mercraft.Core.Unity;

namespace Mercraft.Core.Scene.World.Roads
{
    /// <summary>
    ///     Represents road which can consist of different road elements. This is useful for smooth road rendering
    /// </summary>
    public class Road
    {
        /// <summary>
        ///     Gets or sets game object wrapper which holds game engine specific classes
        /// </summary>
        public IGameObject GameObject { get; set; }

        /// <summary>
        ///     Gets or sets list of road elements
        /// </summary>
        public List<RoadElement> Elements { get; set; }
    }
}