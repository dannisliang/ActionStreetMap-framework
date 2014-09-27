using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;

using Rect = Mercraft.Models.Geometry.Rect;
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

            public string Path { get; set; }

            public IRoofBuilder[] Builders { get; set; }
            public float UnitSize { get; set; }

            public Rect FrontUvMap { get; set; }
            public Rect SideUvMap { get; set; }
        }

        public class FacadeStyle
        {
            public int Height { get; set; }
            public int Width { get; set; }

            public string Material { get; set; }
            public Color32 Color { get; set; }

            public string Path { get; set; }

            public IFacadeBuilder[] Builders { get; set; }

            public Rect FrontUvMap { get; set; }
            public Rect BackUvMap { get; set; }
            public Rect SideUvMap { get; set; }
        }

        #endregion
    }
}
