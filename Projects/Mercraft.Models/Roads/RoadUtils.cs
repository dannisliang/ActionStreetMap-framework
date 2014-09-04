using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Elevation;
using Mercraft.Core.World.Roads;

namespace Mercraft.Models.Roads
{
    public class RoadUtils
    {
        /// <summary>
        ///     Returns road elements which only consist of points in tile.
        ///     Required for non-flat maps.
        /// </summary>
        public static List<RoadElement> GetRoadElementsInTile(HeightMap heightMap, List<RoadElement> roadElements)
        {
            // Current implementation can filter long roads accidentally. Actually, if line which connects two points 
            // crosses more than 1 tile border we can have problems

            var leftBottomCorner = heightMap.LeftBottomCorner;
            var rightUpperCorner = heightMap.RightUpperCorner;
            var result = new List<RoadElement>(roadElements.Count);
            var points = new List<MapPoint>();


            var isNotContinuation = false;
            foreach (var roadElement in roadElements)
            {
                // process all points in this road element
                roadElement.IsNotContinuation = isNotContinuation;
                isNotContinuation = false;
                var isIntersectionSet = false;
                for (int i = 0; i < roadElement.Points.Length; i++)
                {
                    var point = roadElement.Points[i];
                    if (!IsPointInTile(point, leftBottomCorner, rightUpperCorner))
                    {
                        // Point is not in tile. There are two possible further actions:
                        // 1. we have points which are in tile - we should find intersection point with tile border
                        if (points.Any() && !isIntersectionSet)
                        {
                            points.Add(GetIntersectionPoint(points[points.Count - 1], point, leftBottomCorner,
                                rightUpperCorner));
                            isIntersectionSet = true;
                        }
                        // 2. points array is empty - we started from point which isn't part of tile - just skip it
                        isNotContinuation = true;
                    }
                    else
                    {
                        // we left tile and, probably have more than one points which are out of tile
                        // now we back and should find new intersection point to follow direction
                        // previous points should go to different roadElement as we don't want to connect them with current point
                        if (isIntersectionSet)
                        {
                            // copy road element
                            result.Add(CopyRoadElement(roadElement, points));
                            points.Clear();                            
                        }

                        // (!points.Any()) we filtred out points which are located in different tile, so we should 
                        // find intersection point with tile border to render this part
                        if ((isIntersectionSet || !points.Any()) && i != 0)
                        {
                            points.Add(GetIntersectionPoint(point, roadElement.Points[i - 1], leftBottomCorner,
                                rightUpperCorner));
                        }                

                        points.Add(point);
                        isIntersectionSet = false;
                    }
                }

                // if we find any points then we should keep this road element
                if (points.Any())
                {
                    roadElement.Points = points.ToArray(); // assume that we create a copy of this array
                    roadElement.IsNotContinuation = isNotContinuation;
                    result.Add(roadElement);
                }

                // reuse points array
                points.Clear();
            }

            return result;
        }

        private static RoadElement CopyRoadElement(RoadElement roadElement, List<MapPoint> points)
        {
            return new RoadElement()
            {
                Id = roadElement.Id,
                Address = roadElement.Address,
                Lanes = roadElement.Lanes,
                IsNotContinuation = true, //?
                Points = points.ToArray(),
                Width = roadElement.Width
            };
        }

        private static bool IsPointInTile(MapPoint point, MapPoint minPoint, MapPoint maxPoint)
        {
            return point.X >= minPoint.X && point.X <= maxPoint.X &&
                   point.Y >= minPoint.Y && point.Y <= maxPoint.Y;
        }

        /// <summary>
        ///     Find intesection point of segment with tile borders
        /// </summary>
        private static MapPoint GetIntersectionPoint(MapPoint tilePoint, MapPoint nonTilePoint, MapPoint minPoint,
            MapPoint maxPoint)
        {
            // detect the side of tile which intersects with line between points and find its projectionon this side,
            // and tangens of side 
            MapPoint sideProjectionPoint;
            MapPoint axisProjectionPoint;
            float tanAlpha;

            bool isVertical = false;
            // right side
            if (nonTilePoint.X > minPoint.X && nonTilePoint.X > maxPoint.X)
                sideProjectionPoint = new MapPoint(maxPoint.X, tilePoint.Y);
            
            // left side
            else if (nonTilePoint.X < minPoint.X && nonTilePoint.X < maxPoint.X)
                sideProjectionPoint = new MapPoint(minPoint.X, tilePoint.Y);
             
           // top side
            else if (nonTilePoint.Y > minPoint.Y && nonTilePoint.Y > maxPoint.Y)
            {
                sideProjectionPoint = new MapPoint(tilePoint.X, maxPoint.Y);
                isVertical = true;
            }
            // bottom side
            else
            {
                sideProjectionPoint = new MapPoint(tilePoint.X, minPoint.Y);
                isVertical = true;
            }

            axisProjectionPoint = new MapPoint(
                isVertical ? tilePoint.X : nonTilePoint.X,
                isVertical ? nonTilePoint.Y : tilePoint.Y);

            // calculate tangents
            tanAlpha = axisProjectionPoint.DistanceTo(nonTilePoint)/axisProjectionPoint.DistanceTo(tilePoint);

            // calculate distance from side projection point to intersection point
            float distance = tanAlpha*sideProjectionPoint.DistanceTo(tilePoint);

            // should detect sign of offset 
            if (isVertical && tilePoint.X > nonTilePoint.X)
                distance = (-distance);

            if (!isVertical && tilePoint.Y > nonTilePoint.Y)
                distance = (-distance);

            return new MapPoint(
                isVertical ? sideProjectionPoint.X + distance : sideProjectionPoint.X,
                isVertical ? sideProjectionPoint.Y : sideProjectionPoint.Y + distance);
        }
    }
}
