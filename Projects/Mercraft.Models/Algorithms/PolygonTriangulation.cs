using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Algorithms
{
    public class PolygonTriangulation
    {
        public static int[] GetTriangles(Vector2[] verticies)
        {
            var ps = new PolygonShape(verticies);
            ps.CutEar();

            var triangleIndicies = new List<int>();
            for (int i = 0; i < ps.NumberOfPolygons; i++)
            {
                // NOTE: should be always 3!!!
                int nPoints = ps.Polygons(i).Length;
         
                for (int j = 0; j < nPoints; j++)
                {
                    triangleIndicies.Add(Array.FindIndex(verticies, v => v == ps.Polygons(i)[j]));
                }
            }

            return triangleIndicies.ToArray();
        }

        // http://www.codeproject.com/Articles/8238/Polygon-Triangulation-in-C
        #region Helpers entities

        /// <summary>
        /// To define some constant Values used for local judgment 
        /// </summary>
        private struct ConstantValue
        {
            internal const double SmallValue = 0.00001;
   
        }

        /// <summary>
        ///To define a line in the given coordinate system
        ///and related calculations
        ///Line Equation:ax+by+c=0
        ///</summary>

        //a Line in 2D coordinate system: ax+by+c=0
        private class Line
        {
            //line: ax+by+c=0;
            private double a;
            private double b;
            private double c;

            private void Initialize(Double angleInRad, Vector2 point)
            {
                //angleInRad should be between 0-Pi

                try
                {
                    //if ((angleInRad<0) ||(angleInRad>Math.PI))
                    if (angleInRad > 2 * Math.PI)
                    {
                        string errMsg = string.Format("The input line angle" + " {0} is wrong. It should be between 0-2*PI.", angleInRad);
                        throw new InvalidOperationException(errMsg);
                    }

                    if (Math.Abs(angleInRad - Math.PI / 2) <
                        ConstantValue.SmallValue) //vertical line
                    {
                        a = 1;
                        b = 0;
                        c = -point.x;
                    }
                    else //not vertical line
                    {
                        a = -Math.Tan(angleInRad);
                        b = 1;
                        c = -a * point.x - b * point.y;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message + e.StackTrace);
                }
            }

            public Line(Vector2 point1, Vector2 point2)
            {
                try
                {
                    if (PrimitiveExtensions.SamePoint(point1, point2))
                    {
                        throw new ArgumentException("The input points are the same");
                    }

                    //Point1 and Point2 are different points:
                    if (Math.Abs(point1.x - point2.x)
                        < ConstantValue.SmallValue) //vertical line
                    {
                        Initialize(Math.PI / 2, point1);
                    }
                    else if (Math.Abs(point1.y - point2.y)
                        < ConstantValue.SmallValue) //Horizontal line
                    {
                        Initialize(0, point1);
                    }
                    else //normal line
                    {
                        double m = (point2.y - point1.y) / (point2.x - point1.x);
                        double alphaInRad = Math.Atan(m);
                        Initialize(alphaInRad, point1);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message + e.StackTrace);
                }
            }
        }

        private class LineSegment : Line
        {
            //line: ax+by+c=0, with start point and end point
            //direction from start point ->end point
            private Vector2 m_startPoint;
            private Vector2 m_endPoint;

            public Vector2 StartPoint
            {
                get
                {
                    return m_startPoint;
                }
            }

            public Vector2 EndPoint
            {
                get
                {
                    return m_endPoint;
                }
            }

            public LineSegment(Vector2 startPoint, Vector2 endPoint)
                : base(startPoint, endPoint)
            {
                this.m_startPoint = startPoint;
                this.m_endPoint = endPoint;
            }

  

            /*** To calculate the line segment length:   ***/
            public double GetLineSegmentLength()
            {
                double d = (m_endPoint.x - m_startPoint.x) * (m_endPoint.x - m_startPoint.x);
                d += (m_endPoint.y - m_startPoint.y) * (m_endPoint.y - m_startPoint.y);
                d = Math.Sqrt(d);

                return d;
            }

           

            /***Get the minimum x value of the points in the line***/
            public double GetXmin()
            {
                return Math.Min(m_startPoint.x, m_endPoint.x);
            }

            /***Get the maximum  x value of the points in the line***/
            public double GetXmax()
            {
                return Math.Max(m_startPoint.x, m_endPoint.x);
            }

            /***Get the minimum y value of the points in the line***/
            public double GetYmin()
            {
                return Math.Min(m_startPoint.y, m_endPoint.y);
            }

            /***Get the maximum y value of the points in the line***/
            public double GetYmax()
            {
                return Math.Max(m_startPoint.y, m_endPoint.y);
            }

    

            /************************************************
             * Offset the line segment to generate a new line segment
             * If the offset direction is along the x-axis or y-axis, 
             * Parameter is true, other wise it is false
             * ***********************************************/
            /* public LineSegment OffsetLine(double distance, bool rightOrDown)
             {
                 //offset a line with a given distance, generate a new line
                 //rightOrDown=true means offset to x incress direction,
                 // if the line is horizontal, offset to y incress direction

                 LineSegment line;
                 Vector2 newStartPoint = new Vector2();
                 Vector2 newEndPoint = new Vector2();

                 double alphaInRad = this.GetLineAngle(); // 0-PI
                 if (rightOrDown)
                 {
                     if (this.HorizontalLine()) //offset to y+ direction
                     {
                         newStartPoint.x = (float) this.m_startPoint.x;
                         newStartPoint.y = (float)(this.m_startPoint.y + distance);

                         newEndPoint.x = (float) this.m_endPoint.x;
                         newEndPoint.y = (float) (this.m_endPoint.y + distance);
                         line = new LineSegment(newStartPoint, newEndPoint);
                     }
                     else //offset to x+ direction
                     {
                         if (Math.Sin(alphaInRad) > 0)
                         {
                             newStartPoint.x = (float)(m_startPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                             newStartPoint.y = (float) (m_startPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));

                             newEndPoint.x = (float)(m_endPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                             newEndPoint.y = (float) (m_endPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));

                             line = new LineSegment(
                                            newStartPoint, newEndPoint);
                         }
                         else //sin(FalphaInRad)<0
                         {
                             newStartPoint.x = (float) (m_startPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                             newStartPoint.y = (float) (m_startPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));
                             newEndPoint.x = (float) (m_endPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                             newEndPoint.y = (float) (m_endPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));

                             line = new LineSegment(
                                 newStartPoint, newEndPoint);
                         }
                     }
                 }//{rightOrDown}
                 else //leftOrUp
                 {
                     if (this.HorizontalLine()) //offset to y directin
                     {
                         newStartPoint.x = m_startPoint.x;
                         newStartPoint.y = (float)(m_startPoint.y - distance);

                         newEndPoint.x = m_endPoint.x;
                         newEndPoint.y = (float) (m_endPoint.y - distance);
                         line = new LineSegment(newStartPoint, newEndPoint);
                     }
                     else //offset to x directin
                     {
                         if (Math.Sin(alphaInRad) >= 0)
                         {
                             newStartPoint.x = (float)(m_startPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                             newStartPoint.y = (float) (m_startPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));
                             newEndPoint.x = (float) (m_endPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                             newEndPoint.y = (float) (m_endPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));

                             line = new LineSegment(
                                 newStartPoint, newEndPoint);
                         }
                         else //sin(FalphaInRad)<0
                         {
                             newStartPoint.x = (float) (m_startPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                             newStartPoint.y = (float) (m_startPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));
                             newEndPoint.x = (float) (m_endPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                             newEndPoint.y = (float) (m_endPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));

                             line = new LineSegment(
                                 newStartPoint, newEndPoint);
                         }
                     }
                 }
                 return line;
             }*/

            /********************************************************
             To check whether 2 lines segments have an intersection
             *********************************************************/
            /* public bool IntersectedWith(LineSegment line)
             {
                 double x1 = this.m_startPoint.x;
                 double y1 = this.m_startPoint.y;
                 double x2 = this.m_endPoint.x;
                 double y2 = this.m_endPoint.y;
                 double x3 = line.m_startPoint.x;
                 double y3 = line.m_startPoint.y;
                 double x4 = line.m_endPoint.x;
                 double y4 = line.m_endPoint.y;

                 double de = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
                 //if de<>0 then //lines are not parallel
                 if (Math.Abs(de - 0) < ConstantValue.SmallValue) //not parallel
                 {
                     double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / de;
                     double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / de;

                     return ((ub > 0) && (ub < 1));

                 }
                     return false;
             }*/

        }

        private class Polygon
        {

            private Vector2[] m_aVertices;


            public Polygon(Vector2[] points)
            {
                int nNumOfPoitns = points.Length;
                try
                {
                    if (nNumOfPoitns < 3)
                    {
                        throw new ArgumentException("At least 3 points required!");
                    }
                    else
                    {
                        m_aVertices = new Vector2[nNumOfPoitns];
                        for (int i = 0; i < nNumOfPoitns; i++)
                        {
                            m_aVertices[i] = points[i];
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(
                        e.Message + e.StackTrace);
                }
            }

            /***********************************
             From a given point, get its vertex index.
             If the given point is not a polygon vertex, 
             it will return -1 
             ***********************************/
            public int VertexIndex(Vector2 vertex)
            {
                int nIndex = -1;

                int nNumPts = m_aVertices.Length;
                for (int i = 0; i < nNumPts; i++) //each vertex
                {
                    if (PrimitiveExtensions.SamePoint(m_aVertices[i], vertex))
                        nIndex = i;
                }
                return nIndex;
            }

            /***********************************
             From a given vertex, get its previous vertex point.
             If the given point is the first one, 
             it will return  the last vertex;
             If the given point is not a polygon vertex, 
             it will return null; 
             ***********************************/
            public Vector2? PreviousPoint(Vector2 vertex)
            {
                int nIndex;

                nIndex = VertexIndex(vertex);
                if (nIndex == -1)
                    return null;
                else //a valid vertex
                {
                    if (nIndex == 0) //the first vertex
                    {
                        int nPoints = m_aVertices.Length;
                        return m_aVertices[nPoints - 1];
                    }
                    else //not the first vertex
                        return m_aVertices[nIndex - 1];
                }
            }

            /***************************************
                 From a given vertex, get its next vertex point.
                 If the given point is the last one, 
                 it will return  the first vertex;
                 If the given point is not a polygon vertex, 
                 it will return null; 
            ***************************************/
            public Vector2? NextPoint(Vector2 vertex)
            {
                Vector2 nextPt = new Vector2();

                int nIndex;
                nIndex = VertexIndex(vertex);
                if (nIndex == -1)
                    return null;
                else //a valid vertex
                {
                    int nNumOfPt = m_aVertices.Length;
                    if (nIndex == nNumOfPt - 1) //the last vertex
                    {
                        return m_aVertices[0];
                    }
                    else //not the last vertex
                        return m_aVertices[nIndex + 1];
                }
            }




            /******************************************
            To calculate the area of polygon made by given points 

            Good for polygon with holes, but the vertices make the 
            hole  should be in different direction with bounding 
            polygon.
		
            Restriction: the polygon is not self intersecting
            ref: www.swin.edu.au/astronomy/pbourke/
                geometry/polyarea/

            As polygon in different direction, the result coulb be
            in different sign:
            If dblArea>0 : polygon in clock wise to the user 
            If dblArea<0: polygon in count clock wise to the user 		
            *******************************************/
            public static double PolygonArea(Vector2[] points)
            {
                double dblArea = 0;
                int nNumOfPts = points.Length;

                int j;
                for (int i = 0; i < nNumOfPts; i++)
                {
                    j = (i + 1) % nNumOfPts;
                    dblArea += points[i].x * points[j].y;
                    dblArea -= (points[i].y * points[j].x);
                }

                dblArea = dblArea / 2;
                return dblArea;
            }

            /***********************************************
                To check a vertex concave point or a convex point
                -----------------------------------------------------------
                The out polygon is in count clock-wise direction
            ************************************************/
            public VertexType PolygonVertexType(Vector2 vertex)
            {
                VertexType vertexType = VertexType.ErrorPoint;

                if (PolygonVertex(vertex))
                {
                    Vector2 pti = vertex;
                    Vector2 ptj = PreviousPoint(vertex).Value;
                    Vector2 ptk = NextPoint(vertex).Value;

                    double dArea = PolygonArea(new Vector2[] { ptj, pti, ptk });

                    if (dArea < 0)
                        vertexType = VertexType.ConvexPoint;
                    else if (dArea > 0)
                        vertexType = VertexType.ConcavePoint;
                }
                return vertexType;
            }


            
            /*********************************************
            To check whether a given point is a Polygon Vertex
            **********************************************/
            public bool PolygonVertex(Vector2 point)
            {
                bool bVertex = false;
                int nIndex = VertexIndex(point);

                if ((nIndex >= 0) && (nIndex <= m_aVertices.Length - 1))
                    bVertex = true;

                return bVertex;
            }

        


            /*****************************************
            To check given points make a clock-wise polygon or
            count clockwise polygon

            Restriction: the polygon is not self intersecting
            *****************************************/
            public static PolygonDirection PointsDirection(
                Vector2[] points)
            {
                int nCount = 0, j = 0, k = 0;
                int nPoints = points.Length;

                if (nPoints < 3)
                    return PolygonDirection.Unknown;

                for (int i = 0; i < nPoints; i++)
                {
                    j = (i + 1) % nPoints; //j:=i+1;
                    k = (i + 2) % nPoints; //k:=i+2;

                    double crossProduct = (points[j].x - points[i].x)
                        * (points[k].y - points[j].y);
                    crossProduct = crossProduct - (
                        (points[j].y - points[i].y)
                        * (points[k].x - points[j].x)
                        );

                    if (crossProduct > 0)
                        nCount++;
                    else
                        nCount--;
                }

                if (nCount < 0)
                    return PolygonDirection.Count_Clockwise;
                else if (nCount > 0)
                    return PolygonDirection.Clockwise;
                else
                    return PolygonDirection.Unknown;
            }

            /*****************************************************
            To reverse points to different direction (order) :
            ******************************************************/
            public static void ReversePointsDirection(
                Vector2[] points)
            {
                int nVertices = points.Length;
                Vector2[] aTempPts = new Vector2[nVertices];

                for (int i = 0; i < nVertices; i++)
                    aTempPts[i] = points[i];

                for (int i = 0; i < nVertices; i++)
                    points[i] = aTempPts[nVertices - 1 - i];
            }

        }

        private enum PolygonDirection
        {
            Unknown,
            Clockwise,
            Count_Clockwise
        }

        private class PolygonShape
        {
            private Vector2[] m_aInputVertices;
            private Vector2[] m_aUpdatedPolygonVertices;

            private System.Collections.ArrayList m_alEars
                = new System.Collections.ArrayList();
            private Vector2[][] m_aPolygons;

            public int NumberOfPolygons
            {
                get
                {
                    return m_aPolygons.Length;
                }
            }

            public Vector2[] Polygons(int index)
            {
                if (index < m_aPolygons.Length)
                    return m_aPolygons[index];
                else
                    return null;
            }

            public PolygonShape(Vector2[] vertices)
            {
                int nVertices = vertices.Length;
                if (nVertices < 3)
                {
                    System.Diagnostics.Trace.WriteLine("To make a polygon, "
                        + " at least 3 points are required!");
                    return;
                }

                //initalize the 2D points
                m_aInputVertices = new Vector2[nVertices];

                for (int i = 0; i < nVertices; i++)
                    m_aInputVertices[i] = vertices[i];

                //make a working copy,  m_aUpdatedPolygonVertices are
                //in count clock direction from user view
                SetUpdatedPolygonVertices();
            }

            /****************************************************
            To fill m_aUpdatedPolygonVertices array with input array.
		
            m_aUpdatedPolygonVertices is a working array that will 
            be updated when an ear is cut till m_aUpdatedPolygonVertices
            makes triangle (a convex polygon).
           ******************************************************/
            private void SetUpdatedPolygonVertices()
            {
                int nVertices = m_aInputVertices.Length;
                m_aUpdatedPolygonVertices = new Vector2[nVertices];

                for (int i = 0; i < nVertices; i++)
                    m_aUpdatedPolygonVertices[i] = m_aInputVertices[i];

                //m_aUpdatedPolygonVertices should be in count clock wise
                if (Polygon.PointsDirection(m_aUpdatedPolygonVertices)
                    == PolygonDirection.Clockwise)
                    Polygon.ReversePointsDirection(m_aUpdatedPolygonVertices);
            }

            /**********************************************************
            To check the Pt is in the Triangle or not.
            If the Pt is in the line or is a vertex, then return true.
            If the Pt is out of the Triangle, then return false.

            This method is used for triangle only.
            ***********************************************************/
            private bool TriangleContainsPoint(Vector2[] trianglePts, Vector2 pt)
            {
                if (trianglePts.Length != 3)
                    return false;

                for (int i = trianglePts.GetLowerBound(0);
                    i < trianglePts.GetUpperBound(0); i++)
                {
                    if (PrimitiveExtensions.SamePoint(pt, trianglePts[i]))
                        return true;
                }

                bool bIn = false;

                LineSegment line0 = new LineSegment(trianglePts[0], trianglePts[1]);
                LineSegment line1 = new LineSegment(trianglePts[1], trianglePts[2]);
                LineSegment line2 = new LineSegment(trianglePts[2], trianglePts[0]);

                if (PrimitiveExtensions.InLine(pt, line0) ||
                    PrimitiveExtensions.InLine(pt, line1) ||
                    PrimitiveExtensions.InLine(pt, line2))
                    bIn = true;
                else //point is not in the lines
                {
                    double dblArea0 = Polygon.PolygonArea(new Vector2[] { trianglePts[0], trianglePts[1], pt });
                    double dblArea1 = Polygon.PolygonArea(new Vector2[] { trianglePts[1], trianglePts[2], pt });
                    double dblArea2 = Polygon.PolygonArea(new Vector2[] { trianglePts[2], trianglePts[0], pt });

                    if (dblArea0 > 0)
                    {
                        if ((dblArea1 > 0) && (dblArea2 > 0))
                            bIn = true;
                    }
                    else if (dblArea0 < 0)
                    {
                        if ((dblArea1 < 0) && (dblArea2 < 0))
                            bIn = true;
                    }
                }
                return bIn;
            }


            /****************************************************************
            To check whether the Vertex is an ear or not based updated Polygon vertices

            ref. www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian
            /algorithm1.html

            If it is an ear, return true,
            If it is not an ear, return false;
            *****************************************************************/
            private bool IsEarOfUpdatedPolygon(Vector2 vertex)
            {
                Polygon polygon = new Polygon(m_aUpdatedPolygonVertices);

                if (polygon.PolygonVertex(vertex))
                {
                    bool bEar = true;
                    if (polygon.PolygonVertexType(vertex) == VertexType.ConvexPoint)
                    {
                        Vector2 pi = vertex;
                        Vector2 pj = polygon.PreviousPoint(vertex).Value; //previous vertex
                        Vector2 pk = polygon.NextPoint(vertex).Value;//next vertex

                        for (int i = m_aUpdatedPolygonVertices.GetLowerBound(0);
                            i < m_aUpdatedPolygonVertices.GetUpperBound(0); i++)
                        {
                            Vector2 pt = m_aUpdatedPolygonVertices[i];
                            if (!(PrimitiveExtensions.SamePoint(pt, pi) ||
                                PrimitiveExtensions.SamePoint(pt, pj) ||
                                PrimitiveExtensions.SamePoint(pt, pk)))
                            {
                                if (TriangleContainsPoint(new Vector2[] { pj, pi, pk }, pt))
                                    bEar = false;
                            }
                        }
                    } //ThePolygon.getVertexType(Vertex)=ConvexPt
                    else  //concave point
                        bEar = false; //not an ear/
                    return bEar;
                }
                else //not a polygon vertex;
                {
                    System.Diagnostics.Trace.WriteLine("IsEarOfUpdatedPolygon: " +
                        "Not a polygon vertex");
                    return false;
                }
            }

            /****************************************************
            Set up m_aPolygons:
            add ears and been cut Polygon togather
            ****************************************************/
            private void SetPolygons()
            {
                int nPolygon = m_alEars.Count + 1; //ears plus updated polygon
                m_aPolygons = new Vector2[nPolygon][];

                for (int i = 0; i < nPolygon - 1; i++) //add ears
                {
                    Vector2[] points = (Vector2[])m_alEars[i];

                    m_aPolygons[i] = new Vector2[3]; //3 vertices each ear
                    m_aPolygons[i][0] = points[0];
                    m_aPolygons[i][1] = points[1];
                    m_aPolygons[i][2] = points[2];
                }

                //add UpdatedPolygon:
                m_aPolygons[nPolygon - 1] = new
                    Vector2[m_aUpdatedPolygonVertices.Length];

                for (int i = 0; i < m_aUpdatedPolygonVertices.Length; i++)
                {
                    m_aPolygons[nPolygon - 1][i] = m_aUpdatedPolygonVertices[i];
                }
            }

            /********************************************************
            To update m_aUpdatedPolygonVertices:
            Take out Vertex from m_aUpdatedPolygonVertices array, add 3 points
            to the m_aEars
            **********************************************************/
            private void UpdatePolygonVertices(Vector2 vertex)
            {
                System.Collections.ArrayList alTempPts = new System.Collections.ArrayList();

                for (int i = 0; i < m_aUpdatedPolygonVertices.Length; i++)
                {
                    if (PrimitiveExtensions.SamePoint(vertex, m_aUpdatedPolygonVertices[i])) //add 3 pts to FEars
                    {
                        Polygon polygon = new Polygon(m_aUpdatedPolygonVertices);
                        Vector2 pti = vertex;
                        Vector2 ptj = polygon.PreviousPoint(vertex).Value; //previous point
                        Vector2 ptk = polygon.NextPoint(vertex).Value; //next point

                        Vector2[] aEar = new Vector2[3]; //3 vertices of each ear
                        aEar[0] = ptj;
                        aEar[1] = pti;
                        aEar[2] = ptk;

                        m_alEars.Add(aEar);
                    }
                    else
                    {
                        alTempPts.Add(m_aUpdatedPolygonVertices[i]);
                    } //not equal points
                }

                if (m_aUpdatedPolygonVertices.Length
                    - alTempPts.Count == 1)
                {
                    int nLength = m_aUpdatedPolygonVertices.Length;
                    m_aUpdatedPolygonVertices = new Vector2[nLength - 1];

                    for (int i = 0; i < alTempPts.Count; i++)
                        m_aUpdatedPolygonVertices[i] = (Vector2)alTempPts[i];
                }
            }


            /*******************************************************
            To cut an ear from polygon to make ears and an updated polygon:
            *******************************************************/
            public void CutEar()
            {
                bool bFinish = m_aUpdatedPolygonVertices.Length == 3;


                Vector2 pt = new Vector2();
                while (bFinish == false) //UpdatedPolygon
                {
                    int i = 0;
                    bool bNotFound = true;
                    while (bNotFound
                        && (i < m_aUpdatedPolygonVertices.Length)) //loop till find an ear
                    {
                        pt = m_aUpdatedPolygonVertices[i];
                        if (IsEarOfUpdatedPolygon(pt))
                            bNotFound = false; //got one, pt is an ear
                        else
                            i++;
                    } //bNotFount
                    //An ear found:}
                    if (pt != null)
                        UpdatePolygonVertices(pt);

                    if (m_aUpdatedPolygonVertices.Length == 3)
                        bFinish = true;
                } //bFinish=false
                SetPolygons();
            }
        }

        private enum VertexType
        {
            ErrorPoint,
            ConvexPoint,
            ConcavePoint
        }

        private static class PrimitiveExtensions
        {
            public static bool SamePoint(Vector2 point1, Vector2 point2)
            {
                double dDeffX = Math.Abs(point1.x - point2.x);
                double dDeffY = Math.Abs(point1.y - point2.y);

                return (dDeffX < ConstantValue.SmallValue) && (dDeffY < ConstantValue.SmallValue);
            }

            public static bool InLine(Vector2 point, LineSegment lineSegment)
            {
                bool bInline = false;

                double Ax, Ay, Bx, By, Cx, Cy;
                Bx = lineSegment.EndPoint.x;
                By = lineSegment.EndPoint.y;
                Ax = lineSegment.StartPoint.x;
                Ay = lineSegment.StartPoint.y;
                Cx = point.x;
                Cy = point.y;

                double L = lineSegment.GetLineSegmentLength();
                double s = Math.Abs(((Ay - Cy) * (Bx - Ax) - (Ax - Cx) * (By - Ay)) / (L * L));

                if (Math.Abs(s - 0) < ConstantValue.SmallValue)
                {
                    if ((SamePoint(point, lineSegment.StartPoint)) || (SamePoint(point, lineSegment.EndPoint)))
                        bInline = true;
                    else if ((Cx < lineSegment.GetXmax())
                        && (Cx > lineSegment.GetXmin())
                        && (Cy < lineSegment.GetYmax())
                        && (Cy > lineSegment.GetYmin()))
                        bInline = true;
                }
                return bInline;
            }

        }

        #endregion
    }
}
