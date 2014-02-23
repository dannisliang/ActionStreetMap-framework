using UnityEngine;

namespace Mercraft.Models.Primitives
{
    public class PolygonShape
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
                if (pt.SamePoint(trianglePts[i]))
                    return true;
            }

            bool bIn = false;

            CLineSegment line0 = new CLineSegment(trianglePts[0], trianglePts[1]);
            CLineSegment line1 = new CLineSegment(trianglePts[1], trianglePts[2]);
            CLineSegment line2 = new CLineSegment(trianglePts[2], trianglePts[0]);

            if (pt.InLine(line0) || pt.InLine(line1) || pt.InLine(line2))
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
                        if (!(pt.SamePoint(pi) || pt.SamePoint(pj) || pt.SamePoint(pk)))
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
                if (vertex.SamePoint(m_aUpdatedPolygonVertices[i])) //add 3 pts to FEars
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
            Polygon polygon = new Polygon(m_aUpdatedPolygonVertices);
            bool bFinish = false;

            //if (polygon.GetPolygonType()==PolygonType.Convex) //don't have to cut ear
            //	bFinish=true;

            if (m_aUpdatedPolygonVertices.Length == 3) //triangle, don't have to cut ear
                bFinish = true;

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

                polygon = new Polygon(m_aUpdatedPolygonVertices);
                //if ((polygon.GetPolygonType()==PolygonType.Convex)
                //	&& (m_aUpdatedPolygonVertices.Length==3))
                if (m_aUpdatedPolygonVertices.Length == 3)
                    bFinish = true;
            } //bFinish=false
            SetPolygons();
        }
    }
}
