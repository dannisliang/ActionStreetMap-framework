using Mercraft.Infrastructure.Primitives;
using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using Mercraft.Models.Geometry;
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
            public string Type;
            public float Height;
            public string Material;
            public Color32 Color;

            public string Path;

            public IRoofBuilder[] Builders;
            public float UnitSize;

            public Rect FrontUvMap;
            public Rect SideUvMap;
        }

        public class FacadeStyle
        {
            public int Height { get; set; }
            public int Width { get; set; }


            public string Material;
            public Color32 Color;

            public string Path;

            public IFacadeBuilder[] Builders;

            public Rect FrontUvMap;
            public Rect BackUvMap;
            public Rect SideUvMap;
        }

        #endregion
    }
}
