using System;
using UnityEngine;

namespace Mercraft.Models.Geometry
{
    public class Segment
    {
        public Vector3 Start;
        public Vector3 End;

        public Segment(Vector3 start, Vector3 end)
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
