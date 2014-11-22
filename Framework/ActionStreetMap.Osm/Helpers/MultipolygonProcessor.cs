using System;
using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Osm.Entities;
using Area = ActionStreetMap.Core.Scene.Models.Area;

namespace ActionStreetMap.Osm.Helpers
{
    /// <summary>
    ///     Implements algorithm to build Model areas from multipolygon relation
    /// </summary>
    internal static class MultipolygonProcessor
    {
        /// <summary>
        ///     Fills area list by processing multipolygons from given relation
        /// </summary>
        /// <param name="relation">Relation</param>
        /// <param name="areas">List of areas</param>
        public static void FillAreas(Relation relation, List<Area> areas)
        {
            string actualValue;
            if (relation.Tags == null || !relation.Tags.TryGetValue("type", out actualValue) ||
                actualValue != "multipolygon")
                return;

            // see http://wiki.openstreetmap.org/wiki/Relation:multipolygon/Algorithm
            bool allClosed = true;
            int memberCount = relation.Members.Count;
            var outerIndecies = new List<int>(memberCount/2);
            var innerIndecies = new List<int>(memberCount/2);
            var sequences = new List<NodeSequence>(relation.Members.Count / 2);
            foreach (var member in relation.Members)
            {
                var way = member.Member as Way;
                if (way == null || !way.Nodes.Any())
                    continue;
                if (member.Role == "outer")
                    outerIndecies.Add(sequences.Count);
                else if (member.Role == "inner")
                    innerIndecies.Add(sequences.Count);
                else 
                    continue;
              
                var sequence = new NodeSequence(way);
                if (!sequence.IsClosed)
                    allClosed = false;
                sequences.Add(sequence);
            }

            if (outerIndecies.Count == 1 && allClosed)
                SimpleCase(relation, areas, sequences, outerIndecies, innerIndecies);
            else
                ComplexCase(relation, areas, sequences);
        }

        private static void SimpleCase(Relation relation, List<Area> areas, List<NodeSequence> sequences,
            List<int> outerIndecies, List<int> innerIndecies)
        {
            // TODO set correct tags!
            var outer = sequences[outerIndecies[0]];
            areas.Add(new Area()
            {
                Tags = GetTags(relation, outer),
                Points = outer.Coordinates,
                Holes = innerIndecies.Select(i => sequences[i].Coordinates).ToList()
            });
        }

        private static void ComplexCase(Relation relation, List<Area> areas, List<NodeSequence> sequences)
        {
            // TODO set correct tags!
            var rings = CreateRings(sequences);
            if (rings == null)
                return;
            FillAreas(relation, rings, areas);
        }

        private static List<NodeSequence> CreateRings(List<NodeSequence> sequences)
        {
            var closedRings = new List<NodeSequence>();
            NodeSequence currentRing = null;
            while (sequences.Any())
            {
                if (currentRing == null)
                {
                    // start a new ring with any remaining node sequence
                    var lastIndex = sequences.Count - 1;
                    currentRing = sequences[lastIndex];
                    sequences.RemoveAt(lastIndex);
                }
                else
                {
                    // try to continue the ring by appending a node sequence
                    NodeSequence assignedSequence = null;
                    foreach (NodeSequence sequence in sequences)
                    {
                        if (currentRing.TryAdd(sequence))
                        {
                            assignedSequence = sequence;
                            break;
                        }
                    }

                    if (assignedSequence != null)
                        sequences.Remove(assignedSequence);
                    else
                        return null;
                }

                // check whether the ring under construction is closed
                if (currentRing != null && currentRing.IsClosed)
                {
                    // TODO check that it isn't self-intersecting!
                    closedRings.Add(new NodeSequence(currentRing));
                    currentRing = null;
                }
            }

            return currentRing != null ? null : closedRings;
        }

        private static void FillAreas(Relation relation, List<NodeSequence> rings, List<Area> areas)
        {
            while (rings.Any())
            {
                // find an outer ring
                NodeSequence outer = null;
                foreach (NodeSequence candidate in rings)
                {
                    bool containedInOtherRings = false;
                    foreach (NodeSequence other in rings)
                    {
                        if (other != candidate && other.ContainsRing(candidate))
                        {
                            containedInOtherRings = true;
                            break;
                        }
                    }

                    if (!containedInOtherRings)
                    {
                        outer = candidate;
                        break;
                    }
                }

                // find inner rings of that ring
                var inners = new List<NodeSequence>();
                foreach (NodeSequence ring in rings)
                {
                    if (ring != outer && outer.ContainsRing(ring))
                    {
                        bool containedInOthers = false;
                        foreach (NodeSequence other in rings)
                        {
                            if (other != ring && other != outer && other.ContainsRing(ring))
                            {
                                containedInOthers = true;
                                break;
                            }
                        }

                        if (!containedInOthers)
                            inners.Add(ring);
                    }
                }

                // create a new area and remove the used rings
                var holes = new List<List<GeoCoordinate>>(inners.Count);
                foreach (NodeSequence innerRing in inners)
                    holes.Add(innerRing.Coordinates);

                var area = new Area()
                {
                    Id = relation.Id,
                    Tags = GetTags(relation, outer),
                    Points = outer.Coordinates,
                    Holes = holes
                };

                areas.Add(area);

                rings.Remove(outer);
                // remove all innerRings
                foreach (var nodeSequence in inners)
                    rings.Remove(nodeSequence);
            }
        }

        private static Dictionary<string, string> GetTags(Relation relation, NodeSequence outer)
        {
            // TODO tag processing
            return relation.Tags.Count > 1 ? relation.Tags : outer.Tags;
        }

        #region Nested classes

        private class NodeSequence
        {
            private readonly List<Node> _nodes;
            private List<GeoCoordinate> _coordinates;

            public Dictionary<string, string> Tags { get; set; } 

            public NodeSequence(Way way)
            {
                _nodes = new List<Node>(way.Nodes);
                Tags = way.Tags;
            }

            public NodeSequence(NodeSequence sequence)
            {
                _nodes = new List<Node>(sequence._nodes);
            }

            private void AddAll(NodeSequence other) { _nodes.AddRange(other._nodes); }

            private void AddAll(int index, NodeSequence other) { _nodes.InsertRange(index, other._nodes); }

            private void Reverse() { _nodes.Reverse(); }

            /// <summary>
            ///  Tries to add another sequence onto the start or end of this one.
            ///  If it succeeds, the other sequence may also be modified and
            ///  should be considered "spent".
            /// </summary>
            /// <param name="other">NodeSequence.</param>
            public bool TryAdd(NodeSequence other)
            {
                if (LastNode.Coordinate == other.FirstNode.Coordinate)
                {
                    //add the sequence at the end
                    _nodes.RemoveAt(_nodes.Count - 1);
                    AddAll(other);
                    MergeTags(other.Tags);
                    return true;
                }
                if (LastNode.Coordinate == other.LastNode.Coordinate)
                {
                    //add the sequence backwards at the end
                    _nodes.RemoveAt(_nodes.Count - 1);
                    other.Reverse();
                    AddAll(other);
                    MergeTags(other.Tags);
                    return true;
                }
                if (FirstNode.Coordinate == other.LastNode.Coordinate)
                {
                    //add the sequence at the beginning
                    _nodes.RemoveAt(0);
                    AddAll(0, other);
                    MergeTags(other.Tags);
                    return true;
                }
                if (FirstNode.Coordinate == other.FirstNode.Coordinate)
                {
                    //add the sequence backwards at the beginning
                    _nodes.RemoveAt(0);
                    other.Reverse();
                    AddAll(0, other);
                    MergeTags(other.Tags);
                    return true;
                }
                return false;
            }

            private Node FirstNode { get { return _nodes.First(); } }

            private Node LastNode { get { return _nodes.Last(); } }

            public bool IsClosed { get { return _nodes.First().Coordinate == _nodes.Last().Coordinate; } }

            public List<GeoCoordinate> Coordinates
            {
                get { return _coordinates ?? (_coordinates = _nodes.Select(n => n.Coordinate).ToList()); }
            }

            public bool ContainsRing(NodeSequence other)
            {
                return other.Coordinates.All(c => IsPointInPolygon(c, Coordinates));
            }

            private void MergeTags(Dictionary<string, string> other)
            {
                Tags = Tags ?? new Dictionary<string, string>();
                if (other != null)
                {
                    foreach (var key in other.Keys)
                    {
                        if (!Tags.ContainsKey(key))
                            Tags.Add(key, other[key]);
                    }
                }
            }

            /// <summary>
            ///     Checks whether point is in polygon. Have this function here so far
            /// </summary>
            private static bool IsPointInPolygon(GeoCoordinate point, List<GeoCoordinate> verts)
            {
                int i, j, nvert = verts.Count;
                bool c = false;
                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {
                    if (((verts[i].Latitude > point.Latitude) != (verts[j].Latitude > point.Latitude)) &&
                     (point.Longitude < (verts[j].Longitude - verts[i].Longitude) * (point.Latitude - verts[i].Latitude) / 
                     (verts[j].Latitude - verts[i].Latitude) + verts[i].Longitude))
                        c = !c;
                }
                return c;
            }
        }

        #endregion
    }
}
