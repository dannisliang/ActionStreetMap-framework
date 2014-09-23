using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using UnityEngine;
using Color32 = Mercraft.Core.Unity.Color32;

namespace Mercraft.Models.Buildings
{
    public class BuildingStyle
    {
        public RoofStyle Roof { get; set; }
        public FacadeStyle Facade { get; set; }

        #region Nested classes

        public class RoofStyle
        {
            public string Type { get; set; }
            public float Height { get; set; }
            public string Material { get; set; }
            public Color32 Color { get; set; }
            public bool AllowSetColor { get; set; }

            public string[] Textures { get; set; }
            public string[] Materials { get; set; }
            public IRoofBuilder[] Builders { get; set; }
            public float UnitSize { get; set; }
        }

        public class FacadeStyle
        {
            public int Floors { get; set; }
            public float Width { get; set; }
            public string Material { get; set; }
            public Color32 Color { get; set; }
            public bool AllowSetColor { get; set; }

            public string[] Textures { get; set; }
            public string[] Materials { get; set; }
            public IFacadeBuilder[] Builders { get; set; }
            public Vector2[] FrontUvMap { get; set; }
            public Vector2[] BackUvMap { get; set; }
            public Vector2[] SideUvMap { get; set; }
        }

        #endregion
    }
}
