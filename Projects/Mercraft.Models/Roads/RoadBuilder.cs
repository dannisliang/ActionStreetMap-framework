using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.World.Roads;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Primitives;
using Mercraft.Models.Geometry;
using Mercraft.Models.Unity;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    /// <summary>
    ///     Builds road using road model
    /// </summary>
    public interface IRoadBuilder
    {
        void Build(Road road, RoadStyle style);
    }

    public class RoadBuilder : IRoadBuilder
    {
        private readonly IResourceProvider _resourceProvider;

        [Dependency]
        public RoadBuilder(IResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public void Build(Road road, RoadStyle style)
        {
            var context = new BuilderContext(road, style);
            var elementsCount = context.Road.Elements.Count;
            for (context.ElementIndex = 0; context.ElementIndex < elementsCount; context.ElementIndex++)
            {
                context.IsLastElement = context.ElementIndex == elementsCount - 1;
                var roadElement = road.Elements[context.ElementIndex];
                ProcessRoadData(context, roadElement);
            }

            CreateMesh(road, style, context);
        }

        protected virtual void CreateMesh(Road road, RoadStyle style, BuilderContext context)
        {
            // TODO so far we support only flat roads, but we needs elevations feature for something else (e.g. water)
            float zIndex = road.Elements[0].ZIndex;

            Mesh mesh = new Mesh();
            mesh.vertices = context.Points.Select(p => new Vector3(p.x, zIndex, p.y)).ToArray();
            mesh.triangles = context.Triangles.ToArray();
            mesh.uv = context.Uv.ToArray();
            mesh.RecalculateNormals();

            var gameObject = road.GameObject.GetComponent<GameObject>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            //gameObject.AddComponent<MeshCollider>();

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = _resourceProvider.GetMatertial(style.MaterialKey);
            renderer.material.mainTexture = _resourceProvider.GetTexture(style.TextureKey);
        }

        #region Segment processing

        protected void ProcessRoadData(BuilderContext context, RoadElement roadElement)
        {
            var roadSegments = GetRoadSegments(roadElement);

            // NOTE Sometimes the road has only one point (wrong pbf file?)
            if (roadSegments.Count == 0)
                return;

            ProcessFirstSegments(context, roadSegments);
            ProcessLastSegment(context, roadSegments, roadElement.Width);
        }

        /// <summary>
        /// Processes first road segments except last one (if roadSegments.Count > 1)
        /// </summary>
        private void ProcessFirstSegments(BuilderContext context, List<RoadSegment> roadSegments)
        {
            var segmentsCount = roadSegments.Count;
            if (segmentsCount == 1)
            {
                AddTrapezoid(context, roadSegments[0].Left, roadSegments[0].Right);
                context.StartPoints = new Tuple<Vector2, Vector2>(roadSegments[0].Right.End, roadSegments[0].Left.End);
            }
            else
            {
                if (context.StartPoints == null)
                    context.StartPoints = new Tuple<Vector2, Vector2>(roadSegments[0].Right.Start, roadSegments[0].Left.Start);

                for (int i = 1; i < segmentsCount; i++)
                {
                    var s1 = roadSegments[i - 1];
                    var s2 = roadSegments[i];
                    switch (GetManeuverType(s1, s2))
                    {
                        case RoadManeuver.Straight:
                            StraightLineCase(context, s1, s2);
                            break;
                        case RoadManeuver.LeftTurn:
                            TurnLeftCase(context, s1, s2);
                            break;
                        case RoadManeuver.RightTurn:
                            TurnRightCase(context, s1, s2);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes last road segment of current RoadElement
        /// </summary>
        private void ProcessLastSegment(BuilderContext context, List<RoadSegment> roadSegments, float width)
        {
            var segmentsCount = roadSegments.Count;
            // NOTE We have to connect last segment with first segment of next road element
            if (!context.IsLastElement)
            {
                var first = roadSegments[segmentsCount - 1];
                var nextRoadElement = context.Road.Elements[context.ElementIndex + 1];
                var second = GetRoadSegment(nextRoadElement.Points[0],
                    nextRoadElement.Points[1], width);

                Vector2 nextIntersectionPoint;
                switch (GetManeuverType(first, second))
                {
                    case RoadManeuver.Straight:
                        AddTrapezoid(context, second.Right.Start, second.Left.Start, second.Left.End, second.Right.End);
                        context.StartPoints = new Tuple<Vector2, Vector2>(first.Right.End, first.Left.End);
                        break;
                    case RoadManeuver.LeftTurn:
                        nextIntersectionPoint = SegmentUtils.IntersectionPoint(first.Left, second.Left);
                        AddTrapezoid(context, context.StartPoints.Item1, context.StartPoints.Item2, 
                            nextIntersectionPoint, first.Right.End);
                        AddTriangle(context, first.Right.End, nextIntersectionPoint, second.Right.Start, true);
                        context.StartPoints = new Tuple<Vector2, Vector2>(second.Right.Start, nextIntersectionPoint);
                        break;
                    case RoadManeuver.RightTurn:
                        nextIntersectionPoint = SegmentUtils.IntersectionPoint(first.Right, second.Right);
                        AddTrapezoid(context, context.StartPoints.Item1, context.StartPoints.Item2, 
                            first.Left.End, nextIntersectionPoint);
                        AddTriangle(context, first.Left.End, nextIntersectionPoint, second.Left.Start, false);
                        context.StartPoints = new Tuple<Vector2, Vector2>(nextIntersectionPoint, second.Left.Start);
                        break;
                }
            }
            else
            {
                var lastSegment = roadSegments[segmentsCount - 1];
                AddTrapezoid(context, context.StartPoints.Item1, context.StartPoints.Item2, 
                    lastSegment.Left.End, lastSegment.Right.End);
            }
        }
        #endregion

        #region Turn/Straight cases
        private void StraightLineCase(BuilderContext context, RoadSegment first, RoadSegment second)
        {
            AddTrapezoid(context, context.StartPoints.Item1, context.StartPoints.Item2, first.Left.End, first.Right.End);
            context.StartPoints = new Tuple<Vector2, Vector2>(first.Right.End, first.Left.End);
        }

        private void TurnRightCase(BuilderContext context, RoadSegment first, RoadSegment second)
        {
            var intersectionPoint = SegmentUtils.IntersectionPoint(first.Right, second.Right);
            AddTrapezoid(context, context.StartPoints.Item1, context.StartPoints.Item2,
                first.Left.End, intersectionPoint);
            AddTriangle(context, first.Left.End, intersectionPoint, second.Left.Start, false);
            context.StartPoints = new Tuple<Vector2, Vector2>(intersectionPoint, second.Left.Start);
        }

        private void TurnLeftCase(BuilderContext context, RoadSegment first, RoadSegment second)
        {
            var intersectionPoint = SegmentUtils.IntersectionPoint(first.Left, second.Left);
            AddTrapezoid(context, context.StartPoints.Item1, context.StartPoints.Item2,
                intersectionPoint, first.Right.End);
            AddTriangle(context, first.Right.End, intersectionPoint, second.Right.Start, true);
            context.StartPoints = new Tuple<Vector2, Vector2>(second.Right.Start, intersectionPoint);
        }
        #endregion

        #region Add shapes
        private void AddTriangle(BuilderContext context, Vector2 first, Vector2 second, Vector2 third, bool invert)
        {
            context.Points.Add(first);
            context.Points.Add(second);
            context.Points.Add(third);

            context.Triangles.AddRange(new int[]
            {
                context.TrisIndex + 0, context.TrisIndex + (invert? 1 : 2), context.TrisIndex + (invert? 2 : 1)
            });
            context.Uv.AddRange(new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
            });
            context.TrisIndex += 3;
        }

        private void AddTrapezoid(BuilderContext context, Segment left, Segment right)
        {
            AddTrapezoid(context, right.Start, left.Start, left.End, right.End);
        }

        private void AddTrapezoid(BuilderContext context, Vector2 rightStart, Vector2 leftStart, Vector2 leftEnd, Vector2 rightEnd)
        {
            context.Points.Add(rightStart);
            context.Points.Add(leftStart);
            context.Points.Add(leftEnd);
            context.Points.Add(rightEnd);

            context.Triangles.AddRange(new[]
            {
                context.TrisIndex + 0, context.TrisIndex + 1, context.TrisIndex + 2,
                context.TrisIndex + 2, context.TrisIndex + 3, context.TrisIndex + 0
            });
            context.TrisIndex += 4;

            var distance = Vector2.Distance(rightStart, rightEnd);
            float tiles = distance / context.Ratio;
            context.Uv.AddRange(new[]
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

        /// <summary>
        ///     Represents builder context. Used to make class stateless
        /// </summary>
        protected class BuilderContext
        {
            public Road Road;
            public List<Vector2> Points = new List<Vector2>();
            public List<int> Triangles = new List<int>();
            public List<Vector2> Uv = new List<Vector2>();

            public float Ratio = 20;
            public int TrisIndex = 0;
            public Tuple<Vector2, Vector2> StartPoints;

            public int ElementIndex;
            public bool IsLastElement;

            public RoadStyle Style;

            public BuilderContext(Road road, RoadStyle style)
            {
                Road = road;
                Style = style;
            }
        }
    }
}
