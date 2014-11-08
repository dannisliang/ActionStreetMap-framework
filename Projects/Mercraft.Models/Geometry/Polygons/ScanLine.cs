using System;
using System.Collections.Generic;
using Mercraft.Core;

namespace Mercraft.Models.Geometry.Polygons
{
    /// <summary>
    ///     Implements simple scan-line algorithm. Code ported from existing java code found in Internet.
    /// </summary>
    public sealed class ScanLine
    {
        private static readonly List<Edge> EdgeBuffer = new List<Edge>(16);
        // Holds all cutpoints from current scanline with the polygon
        private static readonly List<int> List = new List<int>(32);
        
        /// <summary>
        ///     Fills polygon using points.
        /// </summary>
        /// <param name="points">Polygon points.</param>
        /// <param name="fillAction">Fille action.</param>
        public static void FillPolygon(List<MapPoint> points, Action<int, int, int> fillAction)
        {
            // create edges array from polygon vertice vector
            // make sure that first vertice of an edge is the smaller one
            CreateEdges(points);

            // sort all edges by Y coordinate, smallest one first, lousy bubblesort
            Edge tmp;

            for (int i = 0; i < EdgeBuffer.Count - 1; i++)
                for (int j = 0; j < EdgeBuffer.Count - 1; j++)
                {
                    if (EdgeBuffer[j].StartY > EdgeBuffer[j + 1].StartY)
                    {
                        // swap both edges
                        tmp = EdgeBuffer[j];
                        EdgeBuffer[j] = EdgeBuffer[j + 1];
                        EdgeBuffer[j + 1] = tmp;
                    }
                }

            // find biggest Y-coord of all vertices
            int scanlineEnd = 0;
            for (int i = 0; i < EdgeBuffer.Count; i++)
            {
                if (scanlineEnd < EdgeBuffer[i].EndY)
                    scanlineEnd = EdgeBuffer[i].EndY;
            }          

            // scanline starts at smallest Y coordinate
            // move scanline step by step down to biggest one
            for (int scanline = EdgeBuffer[0].StartY; scanline <= scanlineEnd; scanline++)
            {
                List.Clear();

                // loop all edges to see which are cut by the scanline
                for (int i = 0; i < EdgeBuffer.Count; i++)
                {
                    // here the scanline intersects the smaller vertice
                    if (scanline == EdgeBuffer[i].StartY)
                    {
                        if (scanline == EdgeBuffer[i].EndY)
                        {
                            // the current edge is horizontal, so we add both vertices
                            EdgeBuffer[i].Deactivate();
                            List.Add((int)EdgeBuffer[i].CurrentX);
                        }
                        else
                        {
                            EdgeBuffer[i].Activate();
                            // we don't insert it in the _reusableBuffer cause this vertice is also
                            // the (bigger) vertice of another edge and already handled
                        }
                    }

                    // here the scanline intersects the bigger vertice
                    if (scanline == EdgeBuffer[i].EndY)
                    {
                        EdgeBuffer[i].Deactivate();
                        List.Add((int)EdgeBuffer[i].CurrentX);
                    }

                    // here the scanline intersects the edge, so calc intersection point
                    if (scanline > EdgeBuffer[i].StartY && scanline < EdgeBuffer[i].EndY)
                    {
                        EdgeBuffer[i].Update();
                        List.Add((int)EdgeBuffer[i].CurrentX);
                    }
                }

                // now we have to sort our _reusableBuffer with our X-coordinates, ascendend
                for (int i = 0; i < List.Count; i++)
                    for (int j = 0; j < List.Count - 1; j++)
                    {
                        if (List[j] > List[j + 1])
                        {
                            int swaptmp = List[j];
                            List[j] = List[j + 1];
                            List[j + 1] = swaptmp;
                        }
                    }

                if (List.Count < 2 || List.Count % 2 != 0)
                    continue;

                // so fill all line segments on current scanline
                for (int i = 0; i < List.Count; i += 2)
                    fillAction(scanline, List[i], List[i + 1]);
            }

            EdgeBuffer.Clear();
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
                EdgeBuffer.Add(polygon[i].Y < polygon[nextIndex].Y
                    ? new Edge(polygon[i], polygon[nextIndex])
                    : new Edge(polygon[nextIndex], polygon[i]));
            }
        }

        #region Helper types

        /// <summary>
        ///     Represents an edge of polygon.
        /// </summary>
        private class Edge
        {
            /// <summary>
            ///     Start vertice.
            /// </summary>
            public int StartX;
            public int StartY;

            /// <summary>
            ///     End vertice
            /// </summary>
            public int EndX;
            public int EndY;

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
                StartX = (int) Math.Round(a.X);
                StartY = (int) Math.Round(a.Y);

                EndX = (int)Math.Round(b.X);
                EndY = (int)Math.Round(b.Y);

                // M = dy / dx
                M = (StartY - EndY) / (float)(StartX - EndX);
            }

            /// <summary>
            ///     Called when scanline intersects the first vertice of this edge.
            ///     That simply means that the intersection point is this vertice.
            /// </summary>
            public void Activate()
            {
                CurrentX = StartX;
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
                CurrentX = EndX;
            }
        }

        #endregion
    }
}