using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    /// <summary>
    /// This class contains the design values for a specific roof design
    /// </summary>
    public class Roof
    {
        public Roof(string name)
        {
            Name = name;
        }

        public string Name;

        //roof
        public RoofStyle Style;
        public float Height = 2.0f;
        public float Depth = 1.0f;
        public float FloorDepth = 1.0f;
        public int Direction = 0;
        public int SawtoothTeeth = 4;
        public int BarrelSegments = 20;

        //parapet
        public bool Parapet = true;

        public Mesh ParapetDesign;
        public float ParapetDesignWidth = 1.0f;
        public float ParapetHeight = 0.25f;
        public float ParapetFrontDepth = 0.1f;
        public float ParapetBackDepth = 0.2f;

        //dormer
        public bool HasDormers = false;
        public float DormerWidth = 1.25f;
        public float DormerHeight = 0.85f;
        public float DormerRoofHeight = 0.25f;
        public float MinimumDormerSpacing = 0.5f;
        public float DormerHeightRatio = 0.95f;
        public int WallTexture = 0;

        public int[] TextureValues = new int[8] { 2, 2, 2, 0, 0, 1, 0, 2 };

        public int GetTexture()
        {
            return TextureValues[(int) Style];
        }

        public bool IsFlipped()
        {
            // TODO
            return false;
        }
    }
}
