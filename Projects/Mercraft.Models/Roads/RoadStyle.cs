using Color32 = Mercraft.Core.Unity.Color32;
using Rect = Mercraft.Models.Geometry.Rect;

namespace Mercraft.Models.Roads
{
    public class RoadStyle
    {
        public string Path { get; set; }

        // NOTE ignored so far by default RoadBuilder
        public int Height { get; set; }
        public int Width { get; set; }

        public string Material { get; set; }
        public Color32 Color { get; set; }

        public Rect MainUvMap { get; set; }
        public Rect TurnUvMap { get; set; }
    }
}
