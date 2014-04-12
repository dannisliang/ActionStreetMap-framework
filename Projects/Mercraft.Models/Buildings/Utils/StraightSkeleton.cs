namespace Mercraft.Models.Buildings.Utils
{
    using UnityEngine;
    using System.Collections.Generic;

    public class StraightSkeleton
    {
        private static int numberOfPoints;
        private static List<Vector2> returnPoints;//return as a set of Vector2 in triangle sets of three
        private static Vector2[] originalPoints;
        private static List<Vector2> usePoints;
        private static List<Vector2> interiorPoints;//points that make up the ridge
        private static List<float> pointAngles;
        private static List<float> hypsForward = new List<float>();//list of hypontenuse for ordering the points
        private static List<float> hypsBackward = new List<float>();//list of hypontenuse for ordering the points

        //    private static readonly Color RED = new Color(1,0,0,0.5f);
        //    private static readonly Color BLUE = new Color(0,0,1,0.5f);
        //    private static readonly Color GREEN = new Color(0,1,0,0.5f);
        //    private static readonly Color YELLOW = new Color(1,1,0,0.5f);
        //    private static readonly Color MAGENTA = new Color(1,0,1,0.5f);
        //    private static readonly Color CYAN = new Color(0,1,1,0.5f);

        public static Vector2[][] Calculate(Vector2[] source)
        {
            originalPoints = source;
            returnPoints = new List<Vector2>();//return as a set of Vector2 in triangle sets of three

            numberOfPoints = source.Length;
            usePoints = new List<Vector2>(source);
            pointAngles = new List<float>();
            hypsForward = new List<float>();//list of hypontenuse for ordering the points
            hypsBackward = new List<float>();//list of hypontenuse for ordering the points
            interiorPoints = new List<Vector2>();

            for (int i = 0; i < numberOfPoints; i++)
            {
                //prepopulate the sodding lists
                pointAngles.Add(0);
                hypsForward.Add(float.PositiveInfinity);
                hypsBackward.Add(float.PositiveInfinity);
            }

            for (int i = 1; i < numberOfPoints + 1; i++)
                CalculateAngles(i);


            for (int i = 0; i < numberOfPoints; i++)
                CalculateHypotenuse(i);

            //DEBUG DRAW HYP LINES - FWD+BKWD
            /*        for (int i = 0; i < numberOfPoints; i++)
                    {
            //            int ib = (i + 1) % numberOfPoints;
                        Vector2 pa = usePoints[i];
            //            Vector2 pb = usePoints[ib];
                        float aa = pointAngles[i];
            //            float ab = pointAngles[ib];
                        float hypA = hypsForward[i];
                        float hypB = hypsBackward[i];
                        if (!float.IsPositiveInfinity(hypA))
                        {
                            Vector2 arFDir = new Vector2(Mathf.Sin(aa), Mathf.Cos(aa)) * hypA;
                            Debug.DrawLine(new Vector3(pa.x, 0, pa.y), new Vector3(pa.x + arFDir.x, 0, pa.y + arFDir.y), MAGENTA);
                        }
                        if (!float.IsPositiveInfinity(hypB))
                        {
                            Vector2 arBDir = new Vector2(Mathf.Sin(aa), Mathf.Cos(aa)) * hypB;
                            Debug.DrawLine(new Vector3(pa.x, 0, pa.y), new Vector3(pa.x + arBDir.x, 0, pa.y + arBDir.y), CYAN);
                        }
                        //Vector2 brDir = new Vector2(Mathf.Sin(ab), Mathf.Cos(ab)) * hypB;
                        //Debug.DrawLine(new Vector3(pb.x, 0, pb.y), new Vector3(pb.x + brDir.x, 0, pb.y + brDir.y), CYAN);
                    }*/

            CalculateTriangles();

            return new[] { returnPoints.ToArray(), interiorPoints.ToArray() };
        }

        private static void CalculateTriangles()
        {

            int it = 0;
            int numberOfSkeletalPoints = 0;
            int totalOfSkeletalPoints = numberOfPoints;
            while (numberOfSkeletalPoints < totalOfSkeletalPoints - 3)
            {
                //select shortest point
                int pointIndex = SmallestHypIndex();
                if (pointIndex == -1)
                    break;
                int lastPointIndex = pointIndex > 0 ? pointIndex - 1 : numberOfPoints - 1;
                int lastPointIndexB = pointIndex > 1 ? pointIndex - 2 : pointIndex + (numberOfPoints - 2);
                int nextPointIndex = (pointIndex + 1) % numberOfPoints;
                int nextPointIndexB = (pointIndex + 2) % numberOfPoints;

                Vector2 point = usePoints[pointIndex];
                float pointAngle = pointAngles[pointIndex];
                float hypLengthF = hypsForward[pointIndex];
                float hypLengthB = hypsBackward[pointIndex];
                bool forward = hypLengthF < hypLengthB;//if the next point has a smaller size - delete forward
                float useHypLength = forward ? hypsForward[pointIndex] : hypsBackward[pointIndex];

                Vector2 pointDir = new Vector2(Mathf.Sin(pointAngle), Mathf.Cos(pointAngle)) * useHypLength;
                Vector2 newPoint = point + pointDir;

                int indexA = forward ? lastPointIndex : lastPointIndexB;
                int indexB = forward ? pointIndex : lastPointIndex;
                int indexC = forward ? nextPointIndex : pointIndex;
                int indexD = forward ? nextPointIndexB : nextPointIndex;

                Vector2 pointA = usePoints[indexA];
                Vector2 pointB = usePoints[indexB];
                Vector2 pointC = usePoints[indexC];
                Vector2 pointD = usePoints[indexD];

                //            Debug.DrawLine(new Vector3(pointB.x, 0, pointB.y), new Vector3(newPoint.x, 0, newPoint.y), GREEN);
                //            Debug.DrawLine(new Vector3(pointC.x, 0, pointC.y), new Vector3(newPoint.x, 0, newPoint.y), GREEN);
                //remove points

                pointAngles[indexB] = Mathf.LerpAngle(pointAngles[indexB], pointAngles[indexC], 0.5f);

                if (indexB < indexC)
                {
                    usePoints.RemoveAt(indexB);
                    usePoints.RemoveAt(indexB);
                    hypsForward.RemoveAt(indexB);
                    hypsForward.RemoveAt(indexB);
                    hypsBackward.RemoveAt(indexB);
                    hypsBackward.RemoveAt(indexB);
                    pointAngles.RemoveAt(indexB);
                    pointAngles.RemoveAt(indexB);
                }
                else
                {
                    usePoints.RemoveAt(indexC);
                    hypsForward.RemoveAt(indexC);
                    hypsBackward.RemoveAt(indexC);
                    pointAngles.RemoveAt(indexC);
                    indexB--;
                    usePoints.RemoveAt(indexB);
                    hypsForward.RemoveAt(indexB);
                    hypsBackward.RemoveAt(indexB);
                    pointAngles.RemoveAt(indexB);
                }

                usePoints.Insert(indexB, newPoint);
                interiorPoints.Add(newPoint);
                hypsForward.Insert(indexB, float.PositiveInfinity);
                hypsBackward.Insert(indexB, float.PositiveInfinity);
                pointAngles.Insert(indexB, 0);
                numberOfPoints--;

                totalOfSkeletalPoints++;
                numberOfSkeletalPoints += 2;

                if (indexB < indexA) indexA--;
                if (indexB < indexC) indexC--;
                if (indexB < indexD) indexD--;


                //add new point created
                if (numberOfPoints > 3)
                {
                    CalculateAngles(indexB);
                    CalculateHypotenuse(indexA);
                    CalculateHypotenuse(indexB);
                    CalculateHypotenuse(indexC);
                    CalculateHypotenuse(indexD);
                }
                //re calculate points affected

                returnPoints.Add(pointA);
                returnPoints.Add(pointB);
                returnPoints.Add(newPoint);

                returnPoints.Add(pointB);
                returnPoints.Add(pointC);
                returnPoints.Add(newPoint);

                returnPoints.Add(pointC);
                returnPoints.Add(pointD);
                returnPoints.Add(newPoint);

                it++;
                if (it > 32000)
                {
                    //Debug.Log("CalculateTriangles IT error");
                    break;
                }
            }

            returnPoints.Add(usePoints[0]);
            returnPoints.Add(usePoints[1]);
            returnPoints.Add(usePoints[2]);

            //        for(int i = 0; i < numberOfPoints; i++)
            //        {
            //            Vector3 p0 = new Vector3(usePoints[i].x, 0, usePoints[i].y);
            //            int indexPlus = (i + 1) % usePoints.Count;
            //            Vector3 p1 = new Vector3(usePoints[indexPlus].x, 0, usePoints[indexPlus].y);
            //            Debug.DrawLine(p0, p1, YELLOW);

            //            float pointAngleA = pointAngles[i];
            //            float pointAngleB = pointAngles[indexPlus];
            //            float hypLengthA = hypsForward[i];
            //            float hypLengthB = hypsBackward[indexPlus];
            //            if(float.IsPositiveInfinity(hypLengthA) || float.IsPositiveInfinity(hypLengthB))
            //                continue;
            //            Vector3 pointDirA = new Vector3(Mathf.Sin(pointAngleA), 0, Mathf.Cos(pointAngleA)) * hypLengthA;
            //            Vector3 pointDirB = new Vector3(Mathf.Sin(pointAngleB), 0, Mathf.Cos(pointAngleB)) * hypLengthB;
            //
            //            Debug.DrawLine(p0, p0 + pointDirA, CYAN);
            //            Debug.DrawLine(p1, p1 + pointDirB, MAGENTA);

            //        }
        }

        private static int SmallestHypIndex()
        {
            float smallestHyp = float.PositiveInfinity;
            int smallestHypIndex = -1;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float hypA = hypsForward[i];
                float hypB = hypsBackward[i];
                float hypOtherA = hypsBackward[(i + 1) % numberOfPoints];
                float hypOtherB = hypsForward[(i > 0) ? i - 1 : numberOfPoints - 1];

                if (hypA < smallestHyp && !float.IsPositiveInfinity(hypOtherA))
                {
                    smallestHyp = hypA;
                    smallestHypIndex = i;
                }
                if (hypB < smallestHyp && !float.IsPositiveInfinity(hypOtherB))
                {
                    smallestHyp = hypB;
                    smallestHypIndex = i;
                }
            }
            return smallestHypIndex;
        }

        private static void CalculateAngles(int pointIndex)
        {

            int ia = (pointIndex > 0) ? pointIndex - 1 : numberOfPoints - 1;
            int ib = pointIndex % numberOfPoints;
            int ic = (pointIndex + 1) % numberOfPoints;

            Vector2 a = usePoints[ia];
            Vector2 b = usePoints[ib];
            Vector2 c = usePoints[ic];

            Vector2 dirA = a - b;
            Vector2 dirB = c - b;

            float tarad = Vector2.Angle(Vector2.up, dirA);
            if (tarad < 0) tarad += 360;
            tarad = tarad * Mathf.Deg2Rad * Mathf.Sign(Vector2.Dot(Vector2.right, dirA));
            //Vector2 aDir = new Vector2(Mathf.Sin(tarad), Mathf.Cos(tarad));
            Vector2 aDir90 = new Vector2(Mathf.Sin(tarad + Mathf.PI / 2), Mathf.Cos(tarad + Mathf.PI / 2));//for use to determine reflex angle using Dot

            float tbrad = Vector2.Angle(Vector2.up, dirB);
            if (tbrad < 0) tbrad += 360;
            tbrad = tbrad * Mathf.Deg2Rad * Mathf.Sign(Vector2.Dot(Vector2.right, dirB));
            Vector2 bDir = new Vector2(Mathf.Sin(tbrad), Mathf.Cos(tbrad));

            float reflex = Vector2.Dot(aDir90, bDir) > 0 ? 0 : -1;
            float drad = (Mathf.LerpAngle(tarad * Mathf.Rad2Deg, tbrad * Mathf.Rad2Deg, 0.5f) + (reflex * 180)) * (Mathf.Deg2Rad);

            //        Vector3 midPoint = new Vector3(b.x,0,b.y);
            //        Vector3 angPoint = midPoint + new Vector3(Mathf.Sin(drad), 0, Mathf.Cos(drad))*4;
            //        Debug.DrawLine(midPoint, angPoint, YELLOW);
            //        Debug.DrawLine(midPoint, midPoint+Vector3.left, YELLOW);

            pointAngles[ib] = drad;
        }

        private static void CalculateHypotenuse(int pointIndex)
        {
            //        bool drawLine = numberOfPoints == 4;
            int pointIndexB = (pointIndex + 1) % numberOfPoints;
            Vector2 pa = usePoints[pointIndex];
            Vector2 pb = usePoints[pointIndexB];
            Vector2 baseDir = pa - pb;
            float baseLength = Vector2.Distance(pa, pb);
            float aa = pointAngles[pointIndex];
            float ab = pointAngles[pointIndexB];

            float baseAngle = Vector2.Angle(Vector2.up, baseDir);
            if (baseAngle < 0) baseAngle += 360;
            baseAngle = baseAngle * Mathf.Deg2Rad * Mathf.Sign(Vector2.Dot(Vector2.right, baseDir));

            Vector2 aDir = new Vector2(Mathf.Sin(aa + Mathf.PI * 0.5f), Mathf.Cos(aa + Mathf.PI * 0.5f));
            Vector2 bDir = new Vector2(Mathf.Sin(ab), Mathf.Cos(ab));
            float hypDot = Vector2.Dot(aDir, bDir);
            if (hypDot > -0.01f)//reflex - it'll never intersect laddey
                return;

            float relAngA = aa - baseAngle;
            float relAngB = ab - baseAngle;

            //triangulate the adjacent length
            float adjactentLength = (baseLength * Mathf.Sin(relAngA) * Mathf.Sin(relAngB)) / Mathf.Sin(relAngA - relAngB);
            //Vector3 midPoint = new Vector3((pa.x+pb.x)*0.5f,0,(pa.y+pb.y)*0.5f);
            //float relMid = Mathf.LerpAngle(aa, ab, 0.5f);
            //Vector3 adjPoint = midPoint + new Vector3(Mathf.Sin(relMid), 0, Mathf.Cos(relMid)) * adjactentLength;
            //Debug.DrawLine(midPoint, adjPoint, BLUE);

            float hypA = adjactentLength / Mathf.Sin(relAngA);//trig get the hypot
            float hypB = adjactentLength / Mathf.Sin(relAngB);

            //Fast line intersection HERE
            int numberOfOriginalPoints = originalPoints.Length;
            Vector2 pB0A = pa;
            Vector2 pB1ha = pa + new Vector2(Mathf.Sin(aa), Mathf.Cos(aa)) * hypA;
            Vector2 pB0B = pb;
            Vector2 pB1hb = pb + new Vector2(Mathf.Sin(ab), Mathf.Cos(ab)) * hypB;
            bool calculateForward = true;
            bool calculateBackward = true;
            for (int i = 0; i < numberOfOriginalPoints; i++)
            {
                if (i == pointIndex)
                    continue;
                bool skipForard = false, skipBackward = false;
                if (i == ((pointIndex > 0) ? pointIndex - 1 : numberOfPoints - 1))
                    skipForard = true;
                if (i == (pointIndex + 1) % numberOfPoints)
                    skipBackward = true;
                Vector2 pA0 = originalPoints[i];
                Vector2 pA1 = originalPoints[(i + 1) % numberOfOriginalPoints];

                if (pA0 == pB0A || pA1 == pB0A)
                    skipForard = true;
                if (pA0 == pB0B || pA1 == pB0B)
                    skipBackward = true;

                if (calculateForward && !skipForard)
                {
                    if (BuildingUtils.FastLineIntersection(pA0, pA1, pB0A, pB1ha))
                    {
                        //hypontenuse intersects building plan
                        hypsForward[pointIndex] = float.PositiveInfinity;
                        calculateForward = false;
                        //                    if (drawLine) Debug.DrawLine(new Vector3(pB0A.x, 0, pB0A.y), new Vector3(pB1ha.x, 0, pB1ha.y), YELLOW);
                        //                    if (drawLine) Debug.DrawLine(new Vector3(pA0.x, 0, pA0.y), new Vector3(pA1.x, 0, pA1.y), RED);
                    }
                }
                if (calculateBackward && !skipBackward)
                {
                    if (BuildingUtils.FastLineIntersection(pA0, pA1, pB0B, pB1hb))
                    {
                        //hypontenuse intersects building plan
                        hypsBackward[pointIndexB] = float.PositiveInfinity;
                        calculateBackward = false;
                        //                    if (drawLine) Debug.DrawLine(new Vector3(pB0B.x, 0, pB0B.y), new Vector3(pB1hb.x, 0, pB1hb.y), YELLOW);
                        //                    if (drawLine) Debug.DrawLine(new Vector3(pA0.x, 0, pA0.y), new Vector3(pA1.x, 0, pA1.y), RED);
                    }
                }
            }
            if (calculateForward)
            {
                hypsForward[pointIndex] = hypA;
                //            if (drawLine) Debug.DrawLine(new Vector3(pB0A.x, 0, pB0A.y), new Vector3(pB1ha.x, 0, pB1ha.y), BLUE);

            }
            if (calculateBackward)
            {
                hypsBackward[pointIndexB] = hypB;
                //            if (drawLine) Debug.DrawLine(new Vector3(pB0B.x, 0, pB0B.y), new Vector3(pB1hb.x, 0, pB1hb.y), CYAN);
            }
        }
    }
}
