using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Infrastructure.Primitives;

namespace ActionStreetMap.Core.Scene.World.Roads
{
    /// <summary>
    ///     This class handles road element processing.
    /// </summary>
    public static class RoadElementComposer
    {
        /// <summary>
        ///     Composes RoadElement list which consist of list of joined road elements 
        /// </summary>
        public static IEnumerable<List<RoadElement>> Compose(List<RoadElement> roadElements)
        {
            var firstPointMap = GetFirstPointDictionary(roadElements);
            // join last point of current RoadElement with first point if applicable
            var joinedRoadElementIndecies = new HashSet<int>();
            for (int i = 0; i < roadElements.Count; i++)
            {
                if(joinedRoadElementIndecies.Contains(i))
                    continue;
                var resultRoadElements = new List<RoadElement>();

                var roadElement = roadElements[i];
                resultRoadElements.Add(roadElement);
                
                //joinedRoadElementIndecies.Add(i);

                var lastPoint = roadElement.Points.Last();
                var join = true;
                while (join)
                {
                    join = false;
                    if (firstPointMap.ContainsKey(lastPoint))
                    {
                        var reToJoin = firstPointMap[lastPoint];
                            // do not join roads with different width
                            //.Where(re => Math.Abs(re.Item2.Width - roadElement.Width) < float.Epsilon);
                        foreach (var tuple in reToJoin)
                        {
                            // TODO choose the best matched road element using info from address
                            if (!joinedRoadElementIndecies.Contains(tuple.Item1) /*&& 
                                IsCorrectAngleBetween(roadElement, tuple.Item2)*/)
                            {
                                resultRoadElements.Add(tuple.Item2);
                                joinedRoadElementIndecies.Add(tuple.Item1);
                                lastPoint = tuple.Item2.Points.Last();
                                join = true;
                                break;
                            }
                        }
                    }
                }
                yield return resultRoadElements;
            }
        }

        /*/// <summary>
        /// Returns true if angle between road elements is correct for current render algortihm 
        /// </summary>
        private static bool IsCorrectAngleBetween(RoadElement start, RoadElement end)
        {
            var lineSegment1 = new LineSegment(start.Points[start.Points.Length - 2],
                start.Points[start.Points.Length - 1]);
            var lineSegment2 = new LineSegment(end.Points[0], end.Points[1]);

            var angle = lineSegment1.AngleBetween(lineSegment2);

            // NOTE which value is reasonable?
            return true; //Math.Abs(angle) < 10;
        }*/

        private static Dictionary<MapPoint, List<Tuple<int, RoadElement>>> GetFirstPointDictionary(List<RoadElement> roadElements)
        {
            var firstPointMap = new Dictionary<MapPoint, List<Tuple<int, RoadElement>>>();
            for (int i = 0; i < roadElements.Count; i++)
            {
                var roadElement = roadElements[i];

                var firstPoint = roadElement.Points.First();
                if (!firstPointMap.ContainsKey(firstPoint))
                {
                    firstPointMap[firstPoint] = new List<Tuple<int, RoadElement>>();
                }
                firstPointMap[firstPoint].Add(new Tuple<int, RoadElement>(i, roadElement));
            }

            return firstPointMap;
        }
    }
}
