using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class BuildingStyle
    {
        public RoofStyle Roof { get; set; }
        public FacadeStyle Facade { get; set; }
        public int Floors { get; set; }

        #region Nested classes

        public class RoofStyle
        {
            public string Texture { get; set; }
            public string Material { get; set; }
            public IRoofBuilder Builder { get; set; }
            public Vector2[] UvMap { get; set; }
        }

        public class FacadeStyle
        {
            public string Texture { get; set; }
            public string Material { get; set; }
            public IFacadeBuilder Builder { get; set; }
            public Vector2[] FrontUvMap { get; set; }
            public Vector2[] BackUvMap { get; set; }
            public Vector2[] SideUvMap { get; set; }
        }

        #endregion
    }
}
