using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.World.Roads;
using Mercraft.Infrastructure.Primitives;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    /// <summary>
    /// Builds road using road model
    /// </summary>
    public class RoadBuilder
    {
        private Road _road;
        private List<Vector2> _points;
        private List<int> _triangles;
        private List<Vector2> _uv;

        private float _ratio = 20;
        private int _trisIndex = 0;
        private Tuple<Vector2, Vector2> _startPoints;

        private int _elementIndex;
        private bool _isLastElement;

        public RoadBuilder(Road road)
        {
            _road = road;
            _points = new List<Vector2>();
            _triangles = new List<int>();
            _uv = new List<Vector2>();
        }

        public void Build()
        {
            var elementsCount = _road.Elements.Count;
            for (_elementIndex = 0; _elementIndex < elementsCount; _elementIndex++)
            {
                _isLastElement = _elementIndex == elementsCount - 1;
                var roadElement = _road.Elements[_elementIndex];
                ProcessRoadData(roadElement);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = _points.Select(p => new Vector3(p.x, 0, p.y)).ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uv.ToArray();
            mesh.RecalculateNormals();

            var gameObject = _road.GameObject.GetComponent<GameObject>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>(@"Materials/RoadMaterial");
        }

        #region Segment processing

        public void ProcessRoadData(RoadElement roadElement)
        {
            var roadSegments = GetRoadSegments(roadElement);

            ProcessFirstSegments(roadSegments);
            ProcessLastSegment(roadSegments, roadElement.Width);
        }

        /// <summary>
        /// Processes first road segments except last one (if roadSegments.Count > 1)
        /// </summary>
        private void ProcessFirstSegments(List<RoadSegment> roadSegments)
        {
            var segmentsCount = roadSegments.Count;
            if (segmentsCount == 1)
            {
                AddTrapezoid(roadSegments[0].Left, roadSegments[0].Right);
                _startPoints = new Tuple<Vector2, Vector2>(roadSegments[0].Right.End, roadSegments[0].Left.End);
            }
            else
            {
                if (_startPoints == null)
                    _startPoints = new Tuple<Vector2, Vector2>(roadSegments[0].Right.Start, roadSegments[0].Left.Start);

                for (int i = 1; i < segmentsCount; i++)
                {
                    var s1 = roadSegments[i - 1];
                    var s2 = roadSegments[i];
                    switch (GetManeuverType(s1, s2))
                    {
                        case RoadManeuver.Straight:
                            StraightLineCase(s1, s2);
                            break;
                        case RoadManeuver.LeftTurn:
                            TurnLeftCase(s1, s2);
                            break;
                        case RoadManeuver.RightTurn:
                            TurnRightCase(s1, s2);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes last road segment of current RoadElement
        /// </summary>
        private void ProcessLastSegment(List<RoadSegment> roadSegments, float width)
        {
            var segmentsCount = roadSegments.Count;
            // NOTE We have to connect last segment with first segment of next road element
            if (!_isLastElement)
            {
                var first = roadSegments[segmentsCount - 1];
                var nextRoadElement = _road.Elements[_elementIndex + 1];
                var second = GetRoadSegment(nextRoadElement.Points[0],
                    nextRoadElement.Points[1], width);

                Vector2 nextIntersectionPoint;
                switch (GetManeuverType(first, second))
                {
                    case RoadManeuver.Straight:
                        AddTrapezoid(second.Right.Start, second.Left.Start, second.Left.End, second.Right.End);
                        _startPoints = new Tuple<Vector2, Vector2>(first.Right.End, first.Left.End);
                        break;
                    case RoadManeuver.LeftTurn:
                        nextIntersectionPoint = SegmentUtils.IntersectionPoint(first.Left, second.Left);
                        AddTrapezoid(_startPoints.Item1, _startPoints.Item2, nextIntersectionPoint, first.Right.End);
                        AddTriangle(first.Right.End, nextIntersectionPoint, second.Right.Start, true);
                        _startPoints = new Tuple<Vector2, Vector2>(second.Right.Start, nextIntersectionPoint);
                        break;
                    case RoadManeuver.RightTurn:
                        nextIntersectionPoint = SegmentUtils.IntersectionPoint(first.Right, second.Right);
                        AddTrapezoid(_startPoints.Item1, _startPoints.Item2, first.Left.End, nextIntersectionPoint);
                        AddTriangle(first.Left.End, nextIntersectionPoint, second.Left.Start, false);
                        _startPoints = new Tuple<Vector2, Vector2>(nextIntersectionPoint, second.Left.Start);
                        break;
                }
            }
            else
            {
                var lastSegment = roadSegments[segmentsCount - 1];
                AddTrapezoid(_startPoints.Item1, _startPoints.Item2, lastSegment.Left.End, lastSegment.Right.End);
            }
        }
        #endregion

        #region Turn/Straight cases
        private void StraightLineCase(RoadSegment first, RoadSegment second)
        {
            AddTrapezoid(_startPoints.Item1, _startPoints.Item2, first.Left.End, first.Right.End);
            _startPoints = new Tuple<Vector2, Vector2>(first.Right.End, first.Left.End);
        }

        private void TurnRightCase(RoadSegment first, RoadSegment second)
        {
            var intersectionPoint = SegmentUtils.IntersectionPoint(first.Right, second.Right);
            AddTrapezoid(_startPoints.Item1, _startPoints.Item2, first.Left.End, intersectionPoint);
            AddTriangle(first.Left.End, intersectionPoint, second.Left.Start, false);
            _startPoints = new Tuple<Vector2, Vector2>(intersectionPoint, second.Left.Start);
        }

        private void TurnLeftCase(RoadSegment first, RoadSegment second)
        {
            var intersectionPoint = SegmentUtils.IntersectionPoint(first.Left, second.Left);
            AddTrapezoid(_startPoints.Item1, _startPoints.Item2, intersectionPoint, first.Right.End);
            AddTriangle(first.Right.End, intersectionPoint, second.Right.Start, true);
            _startPoints = new Tuple<Vector2, Vector2>(second.Right.Start, intersectionPoint);
        }
        #endregion

        #region Add shapes
        private void AddTriangle(Vector2 first, Vector2 second, Vector2 third, bool invert)
        {
            _points.Add(first);
            _points.Add(second);
            _points.Add(third);

            _triangles.AddRange(new int[]
            {
                _trisIndex + 0, _trisIndex + (invert? 1 : 2), _trisIndex + (invert? 2 : 1)
            });
            _uv.AddRange(new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
            });
            _trisIndex += 3;
        }

        private void AddTrapezoid(Segment left, Segment right)
        {
            AddTrapezoid(right.Start, left.Start, left.End, right.End);
        }

        private void AddTrapezoid(Vector2 rightStart, Vector2 leftStart, Vector2 leftEnd, Vector2 rightEnd)
        {
            _points.Add(rightStart);
            _points.Add(leftStart);
            _points.Add(leftEnd);
            _points.Add(rightEnd);

            _triangles.AddRange(new[]
            {
                _trisIndex + 0, _trisIndex + 1, _trisIndex + 2,
                _trisIndex + 2, _trisIndex + 3, _trisIndex + 0
            });
            _trisIndex += 4;

            var distance = Vector2.Distance(rightStart, rightEnd);
            float tiles = distance / _ratio;
            _uv.AddRange(new[]
            {
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, tiles),
                new Vector2(1, tiles),
            });
        }
        #endregion

        #region Getting segments and turn types
        private List<RoadSegment> GetRoadSegments(RoadElement roadElement)
        {
            var roadSegments = new List<RoadSegment>();
            for (int i = 1; i < roadElement.Points.Length; i++)
            {
                var point1 = roadElement.Points[i - 1];
                var point2 = roadElement.Points[i];

                roadSegments.Add(GetRoadSegment(point1, point2, roadElement.Width));
            }
            return roadSegments;
        }

        private RoadSegment GetRoadSegment(MapPoint point1, MapPoint point2, float width)
        {
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

            return new RoadSegment(leftSegment, rightSegment);
        }

        private RoadManeuver GetManeuverType(RoadSegment first, RoadSegment second)
        {
            if (SegmentUtils.Intersect(first.Left, second.Left))
                return RoadManeuver.LeftTurn;

            if (SegmentUtils.Intersect(first.Right, second.Right))
                return RoadManeuver.RightTurn;

            return RoadManeuver.Straight;
        }
        #endregion

        private enum RoadManeuver
        {
            Straight,
            LeftTurn,
            RightTurn
        }
    }
}
