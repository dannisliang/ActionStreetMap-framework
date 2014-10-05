using System;
using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Geometry
{
    /// <summary>
    ///     Implements simple scan-line algorithm. Code ported from existing java code found in Internet
    /// </summary>
    public sealed class ScanLine
    {
        private static List<Edge> _edgeBuffer = new List<Edge>(16);
        
        public static void FillPolygon(List<MapPoint> points, Action<int, int, int> fillAction)
        {
            // create edges array from polygon vertice vector
            // make sure that first vertice of an edge is the smaller one
            CreateEdges(points);

            // sort all edges by Y coordinate, smallest one first, lousy bubblesort
            Edge tmp;

            for (int i = 0; i < _edgeBuffer.Count - 1; i++)
                for (int j = 0; j < _edgeBuffer.Count - 1; j++)
                {
                    if (_edgeBuffer[j].Start.Y > _edgeBuffer[j + 1].Start.Y)
                    {
                        // swap both edges
                        tmp = _edgeBuffer[j];
                        _edgeBuffer[j] = _edgeBuffer[j + 1];
                        _edgeBuffer[j + 1] = tmp;
                    }
                }

            // find biggest Y-coord of all vertices
            int scanlineEnd = 0;
            for (int i = 0; i < _edgeBuffer.Count; i++)
            {
                if (scanlineEnd < _edgeBuffer[i].End.Y)
                    scanlineEnd = (int)_edgeBuffer[i].End.Y;
            }

            // Holds all cutpoints from current scanline with the polygon
            List<int> list = new List<int>();

            // scanline starts at smallest Y coordinate
            // move scanline step by step down to biggest one
            for (int scanline = (int)_edgeBuffer[0].Start.Y; scanline <= scanlineEnd; scanline++)
            {
                list.Clear();

                // loop all edges to see which are cut by the scanline
                for (int i = 0; i < _edgeBuffer.Count; i++)
                {
                    // here the scanline intersects the smaller vertice
                    if (scanline == _edgeBuffer[i].Start.Y)
                    {
                        if (scanline == _edgeBuffer[i].End.Y)
                        {
                            // the current edge is horizontal, so we add both vertices
                            _edgeBuffer[i].Deactivate();
                            list.Add((int)_edgeBuffer[i].CurrentX);
                        }
                        else
                        {
                            _edgeBuffer[i].Activate();
                            // we don't insert it in the _reusableBuffer cause this vertice is also
                            // the (bigger) vertice of another edge and already handled
                        }
                    }

                    // here the scanline intersects the bigger vertice
                    if (scanline == _edgeBuffer[i].End.Y)
                    {
                        _edgeBuffer[i].Deactivate();
                        list.Add((int)_edgeBuffer[i].CurrentX);
                    }

                    // here the scanline intersects the edge, so calc intersection point
                    if (scanline > _edgeBuffer[i].Start.Y && scanline < _edgeBuffer[i].End.Y)
                    {
                        _edgeBuffer[i].Update();
                        list.Add((int)_edgeBuffer[i].CurrentX);
                    }
                }

                // now we have to sort our _reusableBuffer with our X-coordinates, ascendend
                for (int i = 0; i < list.Count; i++)
                    for (int j = 0; j < list.Count - 1; j++)
                    {
                        if (list[j] > list[j + 1])
                        {
                            int swaptmp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = swaptmp;
                        }
                    }

                if (list.Count < 2 || list.Count % 2 != 0)
                    continue;

                // so fill all line segments on current scanline
                for (int i = 0; i < list.Count; i += 2)
                    fillAction(scanline, list[i], list[i + 1]);
            }

            _edgeBuffer.Clear();
        }

        /// <summary>
        ///     Create from the polygon vertices an array of edges.
        ///     Note that the first vertice of an edge is always the one with the smaller Y coordinate one of both
        /// </summary>
        private static void CreateEdges(List<MapPoint> polygon)
        {
           //new Edge[polygon.Count];
            for (int i = 0; i < polygon.Count; i++)
            {
                var nextIndex = i == polygon.Count - 1 ? 0 : i + 1;
                if (polygon[i].Y < polygon[nextIndex].Y)
                    _edgeBuffer.Add(new Edge(polygon[i], polygon[nextIndex]));
                else
                    _edgeBuffer.Add(new Edge(polygon[nextIndex], polygon[i]));
            }
        }

        #region Helper types

        /// <summary>
        ///     Represents an edge of polygon
        /// </summary>
        private class Edge
        {
            /// <summary>
            ///     Start vertice
            /// </summary>
            public MapPoint Start;

            /// <summary>
            ///     End vertice
            /// </summary>
            public MapPoint End;

            /// <summary>
            ///     Slope
            /// </summary>
            public readonly float M;

            /// <summary>
            ///     X-coord of intersection with scanline
            /// </summary>
            public float CurrentX;

            public Edge(MapPoint a, MapPoint b)
            {
                Start = new MapPoint(a.X, a.Y);
                End = new MapPoint(b.X, b.Y);

                // M = dy / dx
                M = (Start.Y - End.Y) / (float)(Start.X - End.X);
            }

            /// <summary>
            ///     Called when scanline intersects the first vertice of this edge.
            ///     That simply means that the intersection point is this vertice.
            /// </summary>
            public void Activate()
            {
                CurrentX = Start.X;
            }

            /// <summary>
            ///     Update the intersection point from the scanline and this edge.
            ///     Instead of explicitly calculate it we just increment with 1/M every time
            ///     it is intersected by the scanline.
            /// </summary>
            public void Update()
            {
                CurrentX += 1 / M;
            }


            /// <summary>
            ///     Called when scanline intersects the second vertice,
            ///     so the intersection point is exactly this vertice and from now on
            ///     we are done with this edge
            /// </summary>
            public void Deactivate()
            {
                CurrentX = End.X;
            }
        }

        #endregion
    }
}