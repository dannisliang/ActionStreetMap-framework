using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    public class RandomGen
    {

        private uint _seed = 0;
        private uint m_z = 0;
        private uint m_w = 0;

        public RandomGen(uint newSeed)
        {
            _seed = newSeed;
            Reset();
        }

        public void Reset()
        {
            m_z = m_w = _seed;
        }

        public float output
        {
            get
            {
                m_z = 36969 * (m_z & 65535) + (m_z >> 16);
                m_w = 18000 * (m_w & 65535) + (m_w >> 16);
                uint u = (m_z << 16) + m_w;
                float _output = (float)((u + 1.0f) * 2.328306435454494e-10f);
                return _output;
            }
        }

        public float OutputRange(float min, float max)
        {
            return min + output * (max - min);
        }

        public int OutputRange(int min, int max)
        {
            return min + Mathf.RoundToInt(output * (max - min));
        }

        public bool outputBool
        {
            get
            {
                return output < 0.5f;
            }
        }

        public uint newSeed
        {
            get
            {
                return (uint)(output * 10000);
            }
        }

        public uint seed
        {
            get { return _seed; }

            set
            {
                _seed = value;
                Reset();
            }
        }
    }
}
