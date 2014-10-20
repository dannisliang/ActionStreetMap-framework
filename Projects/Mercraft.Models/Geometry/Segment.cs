using System;
using UnityEngine;

namespace Mercraft.Models.Geometry
{
    /// <summary>
    ///     Represent line segment.
    /// </summary>
    public class Segment
    {
        /// <summary>
        ///     Start.
        /// </summary>
        public Vector3 Start;

        /// <summary>
        ///     End.
        /// </summary>
        public Vector3 End;

        /// <summary>
        ///     Creates Segment.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public Segment(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("{0} {1}", Start, End);
        }
    }
}
