using System.Collections.Generic;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    /// <summary>
    ///     Rename to Model
    /// </summary>
    public class Model
    {
        public BuildingStyle Style;
        public TexturePack TexturePack;
        public RandomGenerator RanGen;

        public IEnumerable<Vector2> Footprint;
        public Plan Plan;

        public List<Roof> Roofs = new List<Roof>();
        public List<Texture> Textures = new List<Texture>();

        public float FloorHeight = BuildingMeasurements.FloorHeightMin;
    }
}