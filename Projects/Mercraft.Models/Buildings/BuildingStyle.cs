using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using Rect = Mercraft.Models.Geometry.Primitives.Rect;
using Color32 = Mercraft.Core.Unity.Color32;

namespace Mercraft.Models.Buildings
{
    /// <summary>
    ///     Defines building style.
    /// </summary>
    public class BuildingStyle
    {
        /// <summary>
        ///     Gets or sets roof style
        /// </summary>
        public RoofStyle Roof { get; set; }

        /// <summary>
        ///     Gets or sets facade style
        /// </summary>
        public FacadeStyle Facade { get; set; }

        #region Nested classes

        /// <summary>
        ///     Defines roof style.
        /// </summary>
        public class RoofStyle
        {
            /// <summary>
            ///     Type.
            /// </summary>
            public string Type;

            /// <summary>
            ///     Height.
            /// </summary>
            public float Height;

            /// <summary>
            ///     Material.
            /// </summary>
            public string Material;

            /// <summary>
            ///     Color.
            /// </summary>
            public Color32 Color;

            /// <summary>
            ///     Material path.
            /// </summary>
            public string Path;

            /// <summary>
            ///     Supported roof builders.
            /// </summary>
            public IRoofBuilder[] Builders { get; set; }

            /// <summary>
            ///     Fron uv map.
            /// </summary>
            public Rect FrontUvMap;

            /// <summary>
            ///     Side uv map.
            /// </summary>
            public Rect SideUvMap;
        }

        /// <summary>
        ///     Defines facade style.
        /// </summary>
        public class FacadeStyle
        {
            /// <summary>
            ///     Height.
            /// </summary>
            public int Height;

            /// <summary>
            ///     Width.
            /// </summary>
            public int Width;

            /// <summary>
            ///     Material.
            /// </summary>
            public string Material;

            /// <summary>
            ///     Color.
            /// </summary>
            public Color32 Color;

            /// <summary>
            ///     Material path.
            /// </summary>
            public string Path;

            /// <summary>
            ///     Supported facade builders.
            /// </summary>
            public IFacadeBuilder[] Builders { get; set; }

            /// <summary>
            ///     Front uv map.
            /// </summary>
            public Rect FrontUvMap;

            /// <summary>
            ///     Back uv map.
            /// </summary>
            public Rect BackUvMap;

            /// <summary>
            ///     Side uv map.
            /// </summary>
            public Rect SideUvMap;
        }

        #endregion
    }
}
