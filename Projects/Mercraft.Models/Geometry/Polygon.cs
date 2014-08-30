using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using UnityEngine;

namespace Mercraft.Models.Geometry
{
    public class Polygon
    {
        // TODO store vertices in Vector3 and remove Elevations
        public float[] Elevations { get; private set; }

        public Vector2[] Verticies { get; private set; }

        public Segment[] Segments { get; private set; }

        public Polygon(IEnumerable<MapPoint> verticies)
        {
            Elevations = verticies.Select(v => v.Elevation).ToArray();
            Verticies = verticies.Select(v => new Vector2(v.X, v.Y)).ToArray();
            ComputeSegments();
        }

        public Polygon(IEnumerable<Vector2> verticies)
        {
            Verticies = verticies.ToArray();
            ComputeSegments();
        }

        private void ComputeSegments()
        {
            // NOTE assume clock-wise order
            Segments = new Segment[Verticies.Length];
            for (int i = 0; i < Verticies.Length; i++)
            {
                var endIndex = i == Verticies.Length - 1 ? 0 : i + 1;
                Segments[i] = new Segment(Verticies[i], Verticies[endIndex]);
            }
        }
    }
}
