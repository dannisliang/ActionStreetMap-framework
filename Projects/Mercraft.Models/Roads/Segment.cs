using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class Segment
    {
        public Vector2 Start;
        public Vector2 End;

        public Segment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }
}
