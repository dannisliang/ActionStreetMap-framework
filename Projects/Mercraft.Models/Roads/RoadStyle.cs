using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadStyle
    {
        public string[] Textures { get; set; }
        public string[] Materials { get; set; }

        // NOTE ignored so far by default RoadBuilder
        public TextureUvMap UvMap { get; set; }

        public class TextureUvMap
        {
            public Vector2[] Main { get; set; }
            public Vector2[] Turn { get; set; }
        }
    }
}
