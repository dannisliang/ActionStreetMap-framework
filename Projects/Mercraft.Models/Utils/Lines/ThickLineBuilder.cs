using System;
using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Infrastructure.Primitives;
using Mercraft.Models.Utils.Geometry;
using UnityEngine;

namespace Mercraft.Models.Utils.Lines
{
    public class ThickLineBuilder
    {
        // TODO this value depends on heightmap accuracy
        private const float MaxPointDistance = 4f;

        private HeightMap _heightMap;

        private List<Vector3> _points = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uv = new List<Vector2>();

        // TODO ration depends on texture
        private float _ratio = 20;

        private int _trisIndex = 0;
        private Tuple<Vector3, Vector3> _startPoints;

        private int _elementIndex;
        private bool _isLastElement;

        private readonly HeightMapProcessor _heightMapProcessor = new HeightMapProcessor();

        public void Build<T>(HeightMap heightMap, IEnumerable<LineElement<T>> elements, 
            Action<List<Vector3>, List<int>, List<Vector2>> builder)
        {
            _heightMap = heightMap;

            var lineElements = ThickLineUtils.GetLineElementsInTile(heightMap, elements);
            var elementsCount = lineElements.Count;
            for (_elementIndex = 0; _elementIndex < elementsCount; _elementIndex++)
            {
                _isLastElement = _elementIndex == elementsCount - 1;

                var lineElement = lineElements[_elementIndex];
                // it means that there is no connection to previous line element
                if (lineElement.IsNotContinuation)
                    _startPoints = null;

                ProcessLine(lineElement, lineElements);
            }

            builder(_points, _triangles, _uv);

            // reset state defaults
            _heightMap = null;
            _trisIndex = 0;
            _startPoints = null;
            _isLastElement = false;
            _points.Clear();
            _triangles.Clear();
            _uv.Clear();
        }

        #region Segment processing

        protected void ProcessLine<T>(LineElement<T> lineElement, List<LineElement<T>> lineElements)
        {
            var lineSegments = GetThickSegments(lineElement);

            // NOTE Sometimes the road has only one point (wrong pbf file?)
            if (lineSegments.Count == 0)
                return;

            ProcessFirstSegments(lineSegments);
            ProcessLastSegment(lineElements, lineSegments, lineElement.Width);
        }

        /// <summary>
        ///     Processes first road segments except last one (if LineSegments.Count > 1)
        /// </summary>
        private void ProcessFirstSegments(List<ThickLineSegment> lineSegments)
        {
            var segmentsCount = lineSegments.Count;
            if (segmentsCount == 1)
            {
                AddTrapezoid(lineSegments[0].Left, lineSegments[0].Right);
                _startPoints = new Tuple<Vector3, Vector3>(lineSegments[0].Right.End, lineSegments[0].Left.End);
            }
            else
            {
                if (_startPoints == null)
                    _startPoints = new Tuple<Vector3, Vector3>(lineSegments[0].Right.Start, lineSegments[0].Left.Start);

                for (int i = 1; i < segmentsCount; i++)
                {
                    var s1 = lineSegments[i - 1];
                    var s2 = lineSegments[i];
                    switch (GetManeuverType(s1, s2))
                    {
                        case Maneuver.Straight:
                            StraightLineCase(s1, s2);
                            break;
                        case Maneuver.LeftTurn:
                            TurnLeftCase(s1, s2);
                            break;
                        case Maneuver.RightTurn:
                            TurnRightCase(s1, s2);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Processes last road segment of current RoadElement
        /// </summary>
        private void ProcessLastSegment<T>(List<LineElement<T>> lineElements, List<ThickLineSegment> lineSegments, float width)
        {
            var segmentsCount = lineSegments.Count;

            // We have to connect last segment with first segment of next road element
            if (!_isLastElement)
            {
                var first = lineSegments[segmentsCount - 1];
                var nextRoadElement = lineElements[_elementIndex + 1];

                // NOTE we couldn't connect last segment of current element with next cause it's marked as not continuation
                if(nextRoadElement.IsNotContinuation)
                    return;

                MapPoint secondPoint = _heightMap.IsFlat
                    ? nextRoadElement.Points[1]
                    // we split lineElement to smaller parts in non-flat mode
                    : ThickLineUtils.GetNextIntermediatePoint(_heightMap,
                        nextRoadElement.Points[0],
                        nextRoadElement.Points[1], MaxPointDistance);
                
                var second = GetThickSegment(nextRoadElement.Points[0], secondPoint, width);

                Vector3 nextIntersectionPoint;
                switch (GetManeuverType(first, second))
                {
                    case Maneuver.Straight:
                        AddTrapezoid(second.Right.Start, second.Left.Start, second.Left.End, second.Right.End);
                        _startPoints = new Tuple<Vector3, Vector3>(first.Right.End, first.Left.End);
                        break;
                    case Maneuver.LeftTurn:
                        nextIntersectionPoint = SegmentUtils.IntersectionPoint(first.Left, second.Left);
                        AddTrapezoid(_startPoints.Item1, _startPoints.Item2, 
                            nextIntersectionPoint, first.Right.End);
                        AddTriangle(first.Right.End, nextIntersectionPoint, second.Right.Start, true);
                        _startPoints = new Tuple<Vector3, Vector3>(second.Right.Start, nextIntersectionPoint);
                        break;
                    case Maneuver.RightTurn:
                        nextIntersectionPoint = SegmentUtils.IntersectionPoint(first.Right, second.Right);
                        AddTrapezoid(_startPoints.Item1, _startPoints.Item2, 
                            first.Left.End, nextIntersectionPoint);
                        AddTriangle(first.Left.End, nextIntersectionPoint, second.Left.Start, false);
                        _startPoints = new Tuple<Vector3, Vector3>(nextIntersectionPoint, second.Left.Start);
                        break;
                }
            }
            else
            {
                // TODO do I need this?
                var lastSegment = lineSegments[segmentsCount - 1];
                AddTrapezoid(_startPoints.Item1, _startPoints.Item2, 
                    lastSegment.Left.End, lastSegment.Right.End);
            }
        }
        #endregion

        #region Turn/Straight cases
        private void StraightLineCase(ThickLineSegment first, ThickLineSegment second)
        {
            AddTrapezoid(_startPoints.Item1, _startPoints.Item2, first.Left.End, first.Right.End);
            _startPoints = new Tuple<Vector3, Vector3>(first.Right.End, first.Left.End);
        }

        private void TurnRightCase(ThickLineSegment first, ThickLineSegment second)
        {
            var intersectionPoint = SegmentUtils.IntersectionPoint(first.Right, second.Right);
            AddTrapezoid(_startPoints.Item1, _startPoints.Item2,
                first.Left.End, intersectionPoint);
            AddTriangle(first.Left.End, intersectionPoint, second.Left.Start, false);
            _startPoints = new Tuple<Vector3, Vector3>(intersectionPoint, second.Left.Start);
        }

        private void TurnLeftCase( ThickLineSegment first, ThickLineSegment second)
        {
            var intersectionPoint = SegmentUtils.IntersectionPoint(first.Left, second.Left);
            AddTrapezoid(_startPoints.Item1, _startPoints.Item2,
                intersectionPoint, first.Right.End);
            AddTriangle(first.Right.End, intersectionPoint, second.Right.Start, true);
            _startPoints = new Tuple<Vector3, Vector3>(second.Right.Start, intersectionPoint);
        }
        #endregion

        #region Add shapes
        private void AddTriangle(Vector3 first, Vector3 second, Vector3 third, bool invert)
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

        private void AddTrapezoid(Vector3 rightStart, Vector3 leftStart, Vector3 leftEnd, Vector3 rightEnd)
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

            var distance = Vector3.Distance(rightStart, rightEnd);
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

        private List<ThickLineSegment> GetThickSegments<T>(LineElement<T> lineElement)
        {
            var lineSegments = new List<ThickLineSegment>();
            
            MapPoint[] points;
            if (_heightMap.IsFlat)
                points = lineElement.Points;
            else
            {
                _heightMapProcessor.Recycle(_heightMap);
                // we should add intermediate points between given to follow elevation changes more smooth 
                points = ThickLineUtils.GetIntermediatePoints(_heightMap, lineElement.Points, MaxPointDistance);
                for (int i = 0; i < points.Length - 1; i++)
                    _heightMapProcessor.AdjustLine(points[i], points[i + 1], lineElement.Width);
            }

            for (int i = 1; i < points.Length; i++)
                lineSegments.Add(GetThickSegment(points[i - 1], points[i], lineElement.Width));

            return lineSegments;
        }

        private ThickLineSegment GetThickSegment(MapPoint point1, MapPoint point2, float width)
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

            var leftSegment = new Segment(new Vector3(lX1, point1.Elevation, lY1), new Vector3(lX2, point2.Elevation, lY2));
            var rightSegment = new Segment(new Vector3(rX1, point1.Elevation, rY1), new Vector3(rX2, point2.Elevation, rY2));

            return new ThickLineSegment(leftSegment, rightSegment);
        }

        private Maneuver GetManeuverType(ThickLineSegment first, ThickLineSegment second)
        {
            // just straight line with shared point
            var area = first.Left.Start.x * (first.Left.End.z - second.Left.End.z) +
                       first.Left.End.x * (second.Left.End.z - first.Left.Start.z) +
                       second.Left.End.x * (first.Left.Start.z - first.Left.End.z);
            if (area < 0.1)
                return Maneuver.Straight;

            if (SegmentUtils.Intersect(first.Left, second.Left))
                return Maneuver.LeftTurn;

            if (SegmentUtils.Intersect(first.Right, second.Right))
                return Maneuver.RightTurn;

            return Maneuver.Straight;
        }

        #endregion

        private enum Maneuver
        {
            Straight,
            LeftTurn,
            RightTurn
        }
    }
}
