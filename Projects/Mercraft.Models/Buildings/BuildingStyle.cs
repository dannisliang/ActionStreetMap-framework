using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class BuildingStyle
    {
        public string Texture { get; set; }
        public string Material { get; set; }

        public int Floors { get; set; }

        public TextureUvMap UvMap { get; set; }

        public IRoofBuilder RoofBuilder { get; set; }
        public IFacadeBuilder FacadeBuilder { get; set; }

        public class TextureUvMap
        {
            public Vector2[] Front { get; set; }
            public Vector2[] Back { get; set; }
            public Vector2[] Side { get; set; }
            public Vector2[] Roof { get; set; }
        }
    }
}
