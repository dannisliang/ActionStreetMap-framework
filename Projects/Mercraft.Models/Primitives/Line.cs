using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Primitives
{
    /// <summary>
    ///To define a line in the given coordinate system
    ///and related calculations
    ///Line Equation:ax+by+c=0
    ///</summary>

    //a Line in 2D coordinate system: ax+by+c=0
    public class CLine
    {
        //line: ax+by+c=0;
        protected double a;
        protected double b;
        protected double c;

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


        public CLine(Double angleInRad, Vector2 point)
        {
            Initialize(angleInRad, point);
        }

        public CLine(Vector2 point1, Vector2 point2)
        {
            try
            {
                if (point1.SamePoint(point2))
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

        public CLine(CLine copiedLine)
        {
            this.a = copiedLine.a;
            this.b = copiedLine.b;
            this.c = copiedLine.c;
        }

        /*** calculate the distance from a given point to the line ***/
        public double GetDistance(Vector2 point)
        {
            double x0 = point.x;
            double y0 = point.y;

            double d = Math.Abs(a * x0 + b * y0 + c);
            d = d / (Math.Sqrt(a * a + b * b));

            return d;
        }

        /*** point(x, y) in the line, based on y, calculate x ***/
        public double GetX(double y)
        {
            //if the line is a horizontal line (a=0), it will return a NaN:
            double x;
            try
            {
                if (Math.Abs(a) < ConstantValue.SmallValue) //a=0;
                {
                    throw new InvalidOperationException();
                }

                x = -(b * y + c) / a;
            }
            catch (Exception e)  //Horizontal line a=0;
            {
                x = System.Double.NaN;
                System.Diagnostics.Trace.
                    WriteLine(e.Message + e.StackTrace);
            }

            return x;
        }

        /*** point(x, y) in the line, based on x, calculate y ***/
        public double GetY(double x)
        {
            //if the line is a vertical line, it will return a NaN:
            double y;
            try
            {
                if (Math.Abs(b) < ConstantValue.SmallValue)
                {
                    throw new InvalidOperationException();
                }
                y = -(a * x + c) / b;
            }
            catch (Exception e)
            {
                y = System.Double.NaN;
                System.Diagnostics.Trace.
                    WriteLine(e.Message + e.StackTrace);
            }
            return y;
        }

        /*** is it a vertical line:***/
        public bool VerticalLine()
        {
            if (Math.Abs(b - 0) < ConstantValue.SmallValue)
                return true;
            else
                return false;
        }

        /*** is it a horizontal line:***/
        public bool HorizontalLine()
        {
            if (Math.Abs(a - 0) < ConstantValue.SmallValue)
                return true;
            else
                return false;
        }

        /*** calculate line angle in radian: ***/
        public double GetLineAngle()
        {
            if (b == 0)
            {
                return Math.PI / 2;
            }
            else //b!=0
            {
                double tanA = -a / b;
                return Math.Atan(tanA);
            }
        }

        public bool Parallel(CLine line)
        {
            bool bParallel = false;
            if (this.a / this.b == line.a / line.b)
                bParallel = true;

            return bParallel;
        }

        /**************************************
         Calculate intersection point of two lines
         if two lines are parallel, return null
         * ************************************/
        public Vector2 IntersecctionWith(CLine line)
        {
            Vector2 point = new Vector2();
            double a1 = this.a;
            double b1 = this.b;
            double c1 = this.c;

            double a2 = line.a;
            double b2 = line.b;
            double c2 = line.c;

            if (!(this.Parallel(line))) //not parallen
            {
                point.x = (float) ((c2 * b1 - c1 * b2) / (a1 * b2 - a2 * b1));
                point.y = (float)  ((a1 * c2 - c1 * a2) / (a2 * b2 - a1 * b2));
            }
            return point;
        }
    }

    public class CLineSegment : CLine
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

        public CLineSegment(Vector2 startPoint, Vector2 endPoint)
            : base(startPoint, endPoint)
        {
            this.m_startPoint = startPoint;
            this.m_endPoint = endPoint;
        }

        /*** chagne the line's direction ***/
        public void ChangeLineDirection()
        {
            Vector2 tempPt;
            tempPt = this.m_startPoint;
            this.m_startPoint = this.m_endPoint;
            this.m_endPoint = tempPt;
        }

        /*** To calculate the line segment length:   ***/
        public double GetLineSegmentLength()
        {
            double d = (m_endPoint.x - m_startPoint.x) * (m_endPoint.x - m_startPoint.x);
            d += (m_endPoint.y - m_startPoint.y) * (m_endPoint.y - m_startPoint.y);
            d = Math.Sqrt(d);

            return d;
        }

        /********************************************************** 
            Get point location, using windows coordinate system: 
            y-axes points down.
            Return Value:
            -1:point at the left of the line (or above the line if the line is horizontal)
             0: point in the line segment or in the line segment 's extension
             1: point at right of the line (or below the line if the line is horizontal)    
         ***********************************************************/
        public int GetPointLocation(Vector2 point)
        {
            double Ax, Ay, Bx, By, Cx, Cy;
            Bx = m_endPoint.x;
            By = m_endPoint.y;

            Ax = m_startPoint.x;
            Ay = m_startPoint.y;

            Cx = point.x;
            Cy = point.y;

            if (this.HorizontalLine())
            {
                if (Math.Abs(Ay - Cy) < ConstantValue.SmallValue) //equal
                    return 0;
                else if (Ay > Cy)
                    return -1;   //Y Axis points down, point is above the line
                else //Ay<Cy
                    return 1;    //Y Axis points down, point is below the line
            }
            else //Not a horizontal line
            {
                //make the line direction bottom->up
                if (m_endPoint.y > m_startPoint.y)
                    this.ChangeLineDirection();

                double L = this.GetLineSegmentLength();
                double s = ((Ay - Cy) * (Bx - Ax) - (Ax - Cx) * (By - Ay)) / (L * L);

                //Note: the Y axis is pointing down:
                if (Math.Abs(s - 0) < ConstantValue.SmallValue) //s=0
                    return 0; //point is in the line or line extension
                else if (s > 0)
                    return -1; //point is left of line or above the horizontal line
                else //s<0
                    return 1;
            }
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

        /***Check whether this line is in a longer line***/
        public bool InLine(CLineSegment longerLineSegment)
        {
            return (m_startPoint.InLine(longerLineSegment)) &&
                           (m_endPoint.InLine(longerLineSegment));
        }

        /************************************************
         * Offset the line segment to generate a new line segment
         * If the offset direction is along the x-axis or y-axis, 
         * Parameter is true, other wise it is false
         * ***********************************************/
       /* public CLineSegment OffsetLine(double distance, bool rightOrDown)
        {
            //offset a line with a given distance, generate a new line
            //rightOrDown=true means offset to x incress direction,
            // if the line is horizontal, offset to y incress direction

            CLineSegment line;
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
                    line = new CLineSegment(newStartPoint, newEndPoint);
                }
                else //offset to x+ direction
                {
                    if (Math.Sin(alphaInRad) > 0)
                    {
                        newStartPoint.x = (float)(m_startPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                        newStartPoint.y = (float) (m_startPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));

                        newEndPoint.x = (float)(m_endPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                        newEndPoint.y = (float) (m_endPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));

                        line = new CLineSegment(
                                       newStartPoint, newEndPoint);
                    }
                    else //sin(FalphaInRad)<0
                    {
                        newStartPoint.x = (float) (m_startPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                        newStartPoint.y = (float) (m_startPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));
                        newEndPoint.x = (float) (m_endPoint.x + Math.Abs(distance * Math.Sin(alphaInRad)));
                        newEndPoint.y = (float) (m_endPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));

                        line = new CLineSegment(
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
                    line = new CLineSegment(newStartPoint, newEndPoint);
                }
                else //offset to x directin
                {
                    if (Math.Sin(alphaInRad) >= 0)
                    {
                        newStartPoint.x = (float)(m_startPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                        newStartPoint.y = (float) (m_startPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));
                        newEndPoint.x = (float) (m_endPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                        newEndPoint.y = (float) (m_endPoint.y + Math.Abs(distance * Math.Cos(alphaInRad)));

                        line = new CLineSegment(
                            newStartPoint, newEndPoint);
                    }
                    else //sin(FalphaInRad)<0
                    {
                        newStartPoint.x = (float) (m_startPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                        newStartPoint.y = (float) (m_startPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));
                        newEndPoint.x = (float) (m_endPoint.x - Math.Abs(distance * Math.Sin(alphaInRad)));
                        newEndPoint.y = (float) (m_endPoint.y - Math.Abs(distance * Math.Cos(alphaInRad)));

                        line = new CLineSegment(
                            newStartPoint, newEndPoint);
                    }
                }
            }
            return line;
        }*/

       /********************************************************
        To check whether 2 lines segments have an intersection
        *********************************************************/
       /* public bool IntersectedWith(CLineSegment line)
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
}
