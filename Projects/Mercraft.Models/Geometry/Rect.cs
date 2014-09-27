using UnityEngine;

namespace Mercraft.Models.Geometry
{
    public class Rect
    {
        public Vector2 LeftBottom { get; private set; }
        public Vector2 RightUpper { get; private set; }

        public Rect(Vector2 leftBottom, Vector2 rightUpper)
        {
            LeftBottom = leftBottom;
            RightUpper = rightUpper;
        }
    }
}
