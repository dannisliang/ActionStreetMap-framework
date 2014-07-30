using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadStyle
    {
        public string Texture { get; set; }
        public string Material { get; set; }

        public TextureUvMap UvMap { get; set; }

        public IRoadBuilder Builder { get; set; }

        public class TextureUvMap
        {
            public Vector2[] Main { get; set; }
            public Vector2[] Turn { get; set; }
        }
    }
}
