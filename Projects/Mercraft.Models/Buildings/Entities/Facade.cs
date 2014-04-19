using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class Facade
    {
        public string Name;
        public bool IsPatterned;
        public bool HasWindows = true;
        public Bay SimpleBay = new Bay("Simple Bay");
        public List<int> BayPattern = new List<int> { 0 };

        public Facade(string name)
        {
            Name = name;
        }


        private readonly int[] _textureValues = new int[8] { 0, 0, 0, 0, 1, 0, 0, 0 };
        public int GetColumnTexture()
        {
            return _textureValues[0];
        }

    }
}
