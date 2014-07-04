using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mercraft.Models.Terrain.Roads
{
    public class RoadBuilder
    {
        private readonly Vector2 _terrainPosition;
        private readonly float _widthRatio;
        private readonly float _heightRatio;

        public RoadBuilder(Vector2 terrainPosition, float widthRatio, float heightRatio)
        {
            _terrainPosition = terrainPosition;
            _widthRatio = widthRatio;
            _heightRatio = heightRatio;
        }

        /// <summary>
        ///     Builds AlphaMapElement array from Terrain elements which is consumed by TerrainBuilder
        /// </summary>
        public AlphaMapElement[] Build(Road[] roads)
        {
            // make polygon from line
            var result = new List<AlphaMapElement>(roads.Length);
            foreach (var road in roads)
            {
                var resultPoints = new List<Vector2>(road.Points.Length*2);
                var roadSegments = GetRoadSegments(road, road.Width);

                var leftPoints = GetPointsFromRoadSegments(roadSegments, true);
                var rightPoints = GetPointsFromRoadSegments(roadSegments, false);

                resultPoints.AddRange(leftPoints);
                rightPoints.Reverse();
                resultPoints.AddRange(rightPoints);

                result.Add(new AlphaMapElement
                {
                    ZIndex = road.ZIndex,
                    SplatIndex = road.SplatIndex,
                    // TODO use name in source and map it to index here
                    Points = resultPoints.Select(p =>
                        TerrainUtils.ConvertWorldToTerrain(p, _terrainPosition, _widthRatio, _heightRatio)).ToArray()
                });
            }
            return result.ToArray();
        }

        private List<RoadSegment> GetRoadSegments(Road terrainElement, float width)
        {
            var roadSegments = new List<RoadSegment>();
            for (int i = 1; i < terrainElement.Points.Length; i++)
            {
                var point1 = terrainElement.Points[i - 1];
                var point2 = terrainElement.Points[i];

                float length = point1.DistanceTo(point2);

                float dxLi = (point2.X - point1.X) / length * width;
                float dyLi = (point2.Y - point1.Y) / length * width;

                // segment moved to the left
                float lX1 = point1.X - dyLi;
                float lY1 = point1.Y + dxLi;
                float lX2 = point2.X - dyLi;
                float lY2 = point2.Y + dxLi;

                // segment moved to the right
                float rX1 = point1.X + dyLi;
                float rY1 = point1.Y - dxLi;
                float rX2 = point2.X + dyLi;
                float rY2 = point2.Y - dxLi;

                var leftSegment = new Segment(new Vector2(lX1, lY1), new Vector2(lX2, lY2));
                var rightSegment = new Segment(new Vector2(rX1, rY1), new Vector2(rX2, rY2));

                roadSegments.Add(new RoadSegment(leftSegment, rightSegment));
            }
            return roadSegments;
        }

        private List<Vector2> GetPointsFromRoadSegments(List<RoadSegment> roadSegments, bool useLeft)
        {
            var points = new List<Vector2>();
            if (roadSegments.Count == 1)
            {
                var s = useLeft ? roadSegments[0].Left : roadSegments[0].Right;
                points.Add(s.Start);
                points.Add(s.End);
                return points;
            }

            for (int i = 1; i < roadSegments.Count; i++)
            {
                var s1 = useLeft ? roadSegments[i - 1].Left : roadSegments[i - 1].Right;
                var s2 = useLeft ? roadSegments[i].Left : roadSegments[i].Right;

                if (i == 1)
                {
                    points.Add(s1.Start);
                }

                if (TerrainUtils.Intersect(s1, s2))
                {
                    points.Add(TerrainUtils.IntersectionPoint(s1, s2));
                }
                else
                {
                    points.Add(s1.End);
                    points.Add(s2.Start);
                }

                if (i == roadSegments.Count - 1)
                {
                    points.Add(s2.End);
                }
            }
            return points;
        }
    }
}