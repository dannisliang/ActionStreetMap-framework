using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class BuildingSettings
    {
        public long Seed { get; set; }
        public float Height { get; set; }
        public int Levels { get; set; }
        public TexturePack TexturePack { get; set; }
        public BuildingStyle Style { get; set; }
        public IEnumerable<Vector2> FootPrint { get; set; }

        public string Material { get; set; }
    }
}
