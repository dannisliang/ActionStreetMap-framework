using System;
using System.Collections.Generic;
using Mercraft.Core.Elevation;
using UnityEngine;

namespace Mercraft.Models.Geometry.ThickLine
{
    /// <summary>
    /// Thick line with heigh/width in 3D
    /// </summary>
    public class DimenLineBuilder : ThickLineBuilder
    {
        public float Height { get; set; }

        public DimenLineBuilder(float height)
        {
            Height = height;
        }

        public override void Build(HeightMap heightMap, List<LineElement> elements, Action<List<Vector3>, List<int>, List<Vector2>> builder)
        {
            base.Build(heightMap, elements, (p, t, u) =>
            {
                ProcessLatestFace();
                builder(Points, Triangles, Uv);
            });
        }

        private void ProcessLatestFace()
        {
            if (Points.Count > 1)
            {
                // NOTE we have to add the latest face
                // NOTE assume that top side is added the latest

                var first = Points[Points.Count - 1];
                var second = Points[Points.Count - 2];
                base.AddTrapezoid(first, second,
                    new Vector3(second.x, second.y - Height, second.z),
                    new Vector3(first.x, first.y - Height, first.z));
            }
        }

        protected override void AddTrapezoid(Vector3 rightStart, Vector3 leftStart, Vector3 leftEnd, Vector3 rightEnd)
        {
            // move up original points
            var newRightStart = new Vector3(rightStart.x, rightStart.y + Height, rightStart.z);
            var newLeftStart = new Vector3(leftStart.x, leftStart.y + Height, leftStart.z);
            var newLeftEnd = new Vector3(leftEnd.x, leftEnd.y + Height, leftEnd.z);
            var newRightEnd = new Vector3(rightEnd.x, rightEnd.y + Height, rightEnd.z);

            // front face
            if (Triangles.Count == 0)
                base.AddTrapezoid(leftStart, newLeftStart, newRightStart, rightStart);

            // right side
            base.AddTrapezoid(rightStart, newRightStart, newRightEnd, rightEnd);

            // left side
            base.AddTrapezoid(leftEnd, newLeftEnd, newLeftStart, leftStart);

            // add top
            base.AddTrapezoid(newRightStart, newLeftStart, newLeftEnd, newRightEnd);
        }

        protected override void AddTriangle(Vector3 first, Vector3 second, Vector3 third, bool invert)
        {
            var newFirst = new Vector3(first.x, first.y + Height, first.z);
            var newSecond = new Vector3(second.x, second.y + Height, second.z);
            var newThird = new Vector3(third.x, third.y + Height, third.z);

            // side
            if (invert)
                base.AddTrapezoid(first, newFirst, newThird, third);
            else
                base.AddTrapezoid(third, newThird, newFirst, first);

            // top
            base.AddTriangle(newFirst, newSecond, newThird, invert);
        }
    }
}
