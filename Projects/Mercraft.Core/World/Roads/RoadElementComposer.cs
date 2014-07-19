using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Primitives;

namespace Mercraft.Core.World.Roads
{
    public static class RoadElementComposer
    {
        /// <summary>
        ///     Composes RoadElement collection which consist of list of joined road elements 
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
                joinedRoadElementIndecies.Add(i);

                var lastPoint = roadElement.Points.Last();
                var join = true;
                while (join)
                {
                    join = false;
                    if (firstPointMap.ContainsKey(lastPoint))
                    {
                        var reToJoin = firstPointMap[lastPoint];
                        foreach (var tuple in reToJoin)
                        {
                            if (!joinedRoadElementIndecies.Contains(tuple.Item1))
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
