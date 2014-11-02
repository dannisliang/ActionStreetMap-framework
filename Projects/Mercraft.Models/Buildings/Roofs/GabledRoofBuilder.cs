using System;
using Mercraft.Core.Scene.World.Buildings;
using Mercraft.Models.Geometry;
using Mercraft.Models.Geometry.Polygons;
using UnityEngine;

namespace Mercraft.Models.Buildings.Roofs
{
    /// <summary>
    ///     Builds gabled roof.
    ///     See http://wiki.openstreetmap.org/wiki/Key:roof:shape#Roof
    /// </summary>
    public class GabledRoofBuilder: HippedRoofBuilder
    {
        /// <inheritdoc />
        public override string Name { get { return "gabled"; } }

        /// <inheritdoc />
        public override bool CanBuild(Building building)
        {
            return true;
        }

         /// <inheritdoc />
        public override MeshData Build(Building building, BuildingStyle style)
        {
            // TODO this algorithm is applicable only to 4-vertices polygons
            // TODO simplify polygon
            var count = building.Footprint.Count;
            // NOTE we use hipped in this case
            if (count != 4)
                return base.Build(building, style);

            var roofHeight = building.RoofHeight;

            var polygon = new Polygon(building.Footprint);

            // detect the longest segment
            float length = 0;
            int longestIndex = 0;
            Segment longestSegment = null;
            for (int i = 0; i < count; i++)
            {
                var segment = polygon.Segments[i];
                var segmentLength = segment.GetLength();
                if (segmentLength > length)
                {
                    longestSegment = segment;
                    length = segmentLength;
                    longestIndex = i;
                }
            }
            // detect other segments
            var adjustedNextSegment = polygon.Segments[(longestIndex + 1)%count];
            var adjustedPrevSegment = polygon.Segments[Math.Abs(longestIndex - 1) % count];
            var parallelSegment = polygon.Segments[(longestIndex + 1) % count];

            // get direction vector
            Vector3 ridgeDirection = longestSegment.End - longestSegment.Start;
            ridgeDirection.Normalize();

            // get centroid
            var centroid = PolygonUtils.GetCentroid(building.Footprint);
            var centroidVector = new Vector3(centroid.X, longestSegment.Start.y, centroid.Y);

            // get something like center line
            Vector3 p1 = centroidVector + length * ridgeDirection;
            Vector3 p2 = centroidVector - length * ridgeDirection;
            var centerSegment = new Segment(p1, p2);

            // get intesection points
            var intersectNext = SegmentUtils.IntersectionPoint(adjustedNextSegment, centerSegment);
            intersectNext.Set(intersectNext.x, intersectNext.y + roofHeight, intersectNext.z);
            var intersectPrev = SegmentUtils.IntersectionPoint(adjustedPrevSegment, centerSegment);
            intersectPrev.Set(intersectPrev.x, intersectPrev.y + roofHeight, intersectPrev.z);

            return new MeshData()
            {
                Vertices = GetVerticies(longestSegment, adjustedNextSegment,
                    adjustedPrevSegment, parallelSegment, intersectNext, intersectPrev),
                Triangles = GetTriangles(),
                UV = GetUv(style),
                MaterialKey = style.Roof.Path,
            };
        }

        private Vector3[] GetVerticies(Segment longestSegment, Segment adjustedNextSegment,
            Segment adjustedPrevSegment, Segment parallelSegment, Vector3 intersectNext, Vector3 intersectPrev)
        {
            return new Vector3[]
            {
                longestSegment.Start,
                intersectNext,
                intersectPrev,

                longestSegment.Start,
                longestSegment.End,
                intersectNext,

                adjustedNextSegment.Start,
                adjustedNextSegment.End,
                intersectNext,

                parallelSegment.Start,
                intersectPrev,
                intersectNext,

                parallelSegment.Start,
                parallelSegment.End,
                intersectPrev,

                adjustedPrevSegment.Start,
                adjustedPrevSegment.End,
                intersectPrev,
            };
        }

        private int[] GetTriangles()
        {
            var triangles = new int[18];
            for(int i=0; i < triangles.Length; i++)
            triangles[i] = i;

            return triangles;
        }

        private Vector2[] GetUv(BuildingStyle style)
        {
            var uvMap = new Vector2[18];
            for (int i = 0; i < uvMap.Length; i++)
                uvMap[i] = style.Roof.FrontUvMap.RightUpper;

            return uvMap;
        }
    }
}
