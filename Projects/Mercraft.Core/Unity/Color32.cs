
using System;

namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Defines RGBA color
    /// </summary>
    public struct Color32
    {
        /// <summary>
        ///     Red.
        /// </summary>
        public byte R;

        /// <summary>
        ///     Green.
        /// </summary>
        public byte G;

        /// <summary>
        ///     Blue.
        /// </summary>
        public byte B;

        /// <summary>
        ///     Alpha
        /// </summary>
        public byte A;

        /// <summary>
        ///     Creates color.
        /// </summary>
        /// <param name="r">Red.</param>
        /// <param name="g">Green.</param>
        /// <param name="b">Blue.</param>
        /// <param name="a">Alpha.</param>
        public Color32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        ///     Converts to int reprsentation.
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            int rgb = R;
            rgb = (rgb << 8) + G;
            rgb = (rgb << 8) + B;

            return rgb;
        }

        /// <summary>
        ///     Returns string representation of this object.
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0},{1},{2},{3})", R, G, B, A);
        }
    }
}
