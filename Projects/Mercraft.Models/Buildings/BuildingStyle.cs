using Mercraft.Models.Buildings.Facades;
using Mercraft.Models.Buildings.Roofs;

namespace Mercraft.Models.Buildings
{
    public class BuildingStyle
    {
        public string Texture { get; set; }
        public string Material { get; set; }

        public int Floors { get; set; }

        public BuildingTextureMap TextureMap { get; set; }

        public IRoofBuilder RoofBuilder { get; set; }
        public IFacadeBuilder FacadeBuilder { get; set; }
    }
}
