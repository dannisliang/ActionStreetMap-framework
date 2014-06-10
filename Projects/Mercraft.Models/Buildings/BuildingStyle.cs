
using System.Collections.Generic;
using Mercraft.Infrastructure.Primitives;

namespace Mercraft.Models.Buildings
{
    /// <summary>
    /// Contains style parameters for building generator
    /// </summary>
    public class BuildingStyle
    {
        public string Name { get; set; }
        public string Texture { get; set; }

        public Range<float> BoxSize { get; set; }
        public Range<float> FloorHeight { get; set; }

        public Range<float> RoofHeight { get; set; }
        public Range<float> RoofFaceDepth { get; set; }
        public Range<float> RoofFloorDepth { get; set; }

        public List<string> RoofStyles { get; set; }

        public float ParapetChance { get; set; }
        public Range<float> ParapetHeight { get; set; }
        public Range<float> ParapetWidth { get; set; }
        public Range<float> ParapetFrontDepth { get; set; }
        public Range<float> ParapetBackDepth { get; set; }

        public float DormerChance { get; set; }
        public Range<float> DormerHeight { get; set; }
        public Range<float> DormerWidth { get; set; }
        public Range<float> DormerSpacing { get; set; }
        public Range<float> DormerRoofHeight { get; set; }
    }
}
