using Mercraft.Core.World.Roads;

namespace Mercraft.Core.World
{
    /// <summary>
    ///     Holds all game models to allow special processing on them depending on game needs
    /// </summary>
    public class WorldManager
    {
        public RoadGraph Roads { get; private set; }
    }
}