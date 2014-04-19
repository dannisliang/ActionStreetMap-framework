using System.Collections.Generic;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    /// <summary>
    /// Rename to Model
    /// </summary>
    public class Data
    {

        public BuildingStyle Style;
        public TexturePack TexturePack;
        public RandomGenerator RanGen;

        public IEnumerable<Vector2> Footprint;
        public Plan Plan;

        public List<Facade> Facades = new List<Facade>();
        public List<Roof> Roofs = new List<Roof>();
        public List<Texture> Textures = new List<Texture>();
        public List<Bay> Bays = new List<Bay>();
        public List<Detail> Details = new List<Detail>();
        public bool DrawUnderside = true;
        public float FoundationHeight;
        public bool Illegal;
        public List<int> TexturesNotPacked = new List<int>();
        public List<int> TexturesPacked = new List<int>();
        public Rect[] AtlasCoords;
        public Texture2D TextureAtlas;
        public Texture2D LodTextureAtlas;

        public float FloorHeight = BuildingMeasurements.FloorHeightMin;      
    }
}
