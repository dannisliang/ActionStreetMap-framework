using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadSettings
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Material Material { get; set; }
        public Vector3[] Points { get; set; }
        public Color Color { get; set; }
    }
}
