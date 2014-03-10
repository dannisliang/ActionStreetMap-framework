
using System.Collections.Generic;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.Scene
{
    /// <summary>
    /// Represents map scene
    /// </summary>
    public interface IScene
    {
        void AddArea(Area area);
        void AddWay(Way way);

        IEnumerable<Area> Areas { get; }
        IEnumerable<Way> Ways { get; }
    }
}
