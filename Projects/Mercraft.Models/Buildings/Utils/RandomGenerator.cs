using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    public class RandomGenerator
    {
        private uint _z;
        private uint _w;

        public RandomGenerator(uint seed)
        {
            _z = _w = seed;
        }

        public float Output
        {
            get
            {
                _z = 36969*(_z & 65535) + (_z >> 16);
                _w = 18000*(_w & 65535) + (_w >> 16);
                uint u = (_z << 16) + _w;
                return (u + 1.0f)*2.328306435454494e-10f;
            }
        }

        public float OutputRange(float min, float max)
        {
            return min + Output*(max - min);
        }

        public int OutputRange(int min, int max)
        {
            return min + Mathf.RoundToInt(Output*(max - min));
        }
    }
}