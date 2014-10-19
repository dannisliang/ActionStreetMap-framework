
using System;

namespace Mercraft.Core.Unity
{
    public struct Color32
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public int ToInt()
        {
            int rgb = R;
            rgb = (rgb << 8) + G;
            rgb = (rgb << 8) + B;

            return rgb;
        }

        public override string ToString()
        {
            return String.Format("({0},{1},{2},{3})", R, G, B, A);
        }
    }
}
