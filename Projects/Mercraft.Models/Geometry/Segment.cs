using System;
using UnityEngine;

namespace Mercraft.Models.Geometry
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

        public override string ToString()
        {
            return String.Format("{0} {1}", Start, End);
        }
    }
}
