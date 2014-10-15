using UnityEngine;

namespace Mercraft.Models.Utils
{
    public static class UnityColorUtility
    {
        public static UnityEngine.Color32 FromCore(Core.Unity.Color32 color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        public static bool IsDefault(UnityEngine.Color32 color)
        {
            return color.r == 0 && color.g == 0 && color.b == 0;
        }
    }
}
