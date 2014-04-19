using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class Volume
    {
        public List<int> Points = new List<int>();
        public List<bool> RenderFacade = new List<bool>();

        public float Height = 10;

        public int NumberOfFloors = 1;
        public VolumeStyle Style = new VolumeStyle();
        public int RoofDesignId = 0;

        public void Add(int newInt)
        {
            Points.Add(newInt);
            RenderFacade.Add(true);
            Style.AddStyle(0, newInt, 1);
        }

        public int Count
        {
            get { return Points.Count; }
        }
    }
}
