using System.Collections.Generic;
using Mercraft.Models.Buildings.Enums;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class Data
    {
        public GenerateConstraints GeneratorConstraints { get; set; }

        public IEnumerable<Vector2> Footprint { get; set; }
        public Plan Plan { get; set; }

        public List<FacadeDesign> Facades { get; set; }
        public List<RoofDesign> Roofs { get; set; }
        public List<Texture> Textures { get; set; }
        public List<Bay> Bays { get; set; }
        public List<Detail> Details { get; set; }
        public bool DrawUnderside { get; set; }
        public float FoundationHeight { get; set; }
        public bool Illegal { get; set; }
        public List<int> TexturesNotPacked { get; set; }
        public List<int> TexturesPacked { get; set; }
        public Rect[] AtlasCoords { get; set; }
        public Texture2D TextureAtlas { get; set; }
        public Texture2D LodTextureAtlas { get; set; }

        private float _floorHeight = BuildingMeasurements.FloorHeightMin;

        public Data()
        {
            TexturesPacked = new List<int>();
            TexturesNotPacked = new List<int>();
            Illegal = false;
            FoundationHeight = 0;
            DrawUnderside = true;
            Details = new List<Detail>();
            Bays = new List<Bay>();
            Textures = new List<Texture>();
            Roofs = new List<RoofDesign>();
            Facades = new List<FacadeDesign>();
        }

        public float FloorHeight
        {
            get
            {
                return _floorHeight;
            }
            set
            {
                _floorHeight = value;

                if (_floorHeight > 0)
                {
                    foreach (FacadeDesign facadeDesign in Facades)
                    {
                        facadeDesign.doorHeight = Mathf.Min(facadeDesign.doorHeight, _floorHeight);
                        facadeDesign.windowHeight = Mathf.Min(facadeDesign.windowHeight, _floorHeight);
                    }
                }
            }
        }       
    }
}
