namespace Mercraft.Models.Buildings.Utils
{
    using UnityEngine;
    using System.Collections.Generic;

    public class StraightSkeleton
    {
        public static Vector2[][] Calculate(Vector2[] source)
        {
            var context = new SkeletonContext();
            context.originalPoints = source;
            context.numberOfPoints = source.Length;
            context.usePoints = new List<Vector2>(source);

            for (int i = 0; i < context.numberOfPoints; i++)
            {
                //prepopulate the sodding lists
                context.pointAngles.Add(0);
                context.hypsForward.Add(float.PositiveInfinity);
                context.hypsBackward.Add(float.PositiveInfinity);
            }

            for (int i = 1; i < context.numberOfPoints + 1; i++)
                CalculateAngles(context, i);


            for (int i = 0; i < context.numberOfPoints; i++)
                CalculateHypotenuse(context, i);

            CalculateTriangles(context);

            return new[] { context.returnPoints.ToArray(), context.interiorPoints.ToArray() };
        }

        private static void CalculateTriangles(SkeletonContext context)
        {

            int it = 0;
            int numberOfSkeletalPoints = 0;
            int totalOfSkeletalPoints = context.numberOfPoints;
            while (numberOfSkeletalPoints < totalOfSkeletalPoints - 3)
            {
                //select shortest point
                int pointIndex = SmallestHypIndex(context);
                if (pointIndex == -1)
                    break;
                int lastPointIndex = pointIndex > 0 ? pointIndex - 1 : context.numberOfPoints - 1;
                int lastPointIndexB = pointIndex > 1 ? pointIndex - 2 : pointIndex + (context.numberOfPoints - 2);
                int nextPointIndex = (pointIndex + 1) % context.numberOfPoints;
                int nextPointIndexB = (pointIndex + 2) % context.numberOfPoints;

                Vector2 point = context.usePoints[pointIndex];
                float pointAngle = context.pointAngles[pointIndex];
                float hypLengthF = context.hypsForward[pointIndex];
                float hypLengthB = context.hypsBackward[pointIndex];
                bool forward = hypLengthF < hypLengthB;//if the next point has a smaller size - delete forward
                float useHypLength = forward ? context.hypsForward[pointIndex] : context.hypsBackward[pointIndex];

                Vector2 pointDir = new Vector2(Mathf.Sin(pointAngle), Mathf.Cos(pointAngle)) * useHypLength;
                Vector2 newPoint = point + pointDir;

                int indexA = forward ? lastPointIndex : lastPointIndexB;
                int indexB = forward ? pointIndex : lastPointIndex;
                int indexC = forward ? nextPointIndex : pointIndex;
                int indexD = forward ? nextPointIndexB : nextPointIndex;

                Vector2 pointA = context.usePoints[indexA];
                Vector2 pointB = context.usePoints[indexB];
                Vector2 pointC = context.usePoints[indexC];
                Vector2 pointD = context.usePoints[indexD];

                context.pointAngles[indexB] = Mathf.LerpAngle(context.pointAngles[indexB], context.pointAngles[indexC], 0.5f);

                if (indexB < indexC)
                {
                    context.usePoints.RemoveAt(indexB);
                    context.usePoints.RemoveAt(indexB);
                    context.hypsForward.RemoveAt(indexB);
                    context.hypsForward.RemoveAt(indexB);
                    context.hypsBackward.RemoveAt(indexB);
                    context.hypsBackward.RemoveAt(indexB);
                    context.pointAngles.RemoveAt(indexB);
                    context.pointAngles.RemoveAt(indexB);
                }
                else
                {
                    context.usePoints.RemoveAt(indexC);
                    context.hypsForward.RemoveAt(indexC);
                    context.hypsBackward.RemoveAt(indexC);
                    context.pointAngles.RemoveAt(indexC);
                    indexB--;
                    context.usePoints.RemoveAt(indexB);
                    context.hypsForward.RemoveAt(indexB);
                    context.hypsBackward.RemoveAt(indexB);
                    context.pointAngles.RemoveAt(indexB);
                }

                context.usePoints.Insert(indexB, newPoint);
                context.interiorPoints.Add(newPoint);
                context.hypsForward.Insert(indexB, float.PositiveInfinity);
                context.hypsBackward.Insert(indexB, float.PositiveInfinity);
                context.pointAngles.Insert(indexB, 0);
                context.numberOfPoints--;

                totalOfSkeletalPoints++;
                numberOfSkeletalPoints += 2;

                if (indexB < indexA) indexA--;
                if (indexB < indexC) indexC--;
                if (indexB < indexD) indexD--;


                //add new point created
                if (context.numberOfPoints > 3)
                {
                    CalculateAngles(context,indexB);
                    CalculateHypotenuse(context, indexA);
                    CalculateHypotenuse(context, indexB);
                    CalculateHypotenuse(context, indexC);
                    CalculateHypotenuse(context, indexD);
                }
                //re calculate points affected

                context.returnPoints.Add(pointA);
                context.returnPoints.Add(pointB);
                context.returnPoints.Add(newPoint);

                context.returnPoints.Add(pointB);
                context.returnPoints.Add(pointC);
                context.returnPoints.Add(newPoint);

                context.returnPoints.Add(pointC);
                context.returnPoints.Add(pointD);
                context.returnPoints.Add(newPoint);

                it++;
                if (it > 32000)
                {
                    break;
                }
            }

            context.returnPoints.Add(context.usePoints[0]);
            context.returnPoints.Add(context.usePoints[1]);
            context.returnPoints.Add(context.usePoints[2]);

        }

        private static int SmallestHypIndex(SkeletonContext context)
        {
            float smallestHyp = float.PositiveInfinity;
            int smallestHypIndex = -1;
            for (int i = 0; i < context.numberOfPoints; i++)
            {
                float hypA = context.hypsForward[i];
                float hypB = context.hypsBackward[i];
                float hypOtherA = context.hypsBackward[(i + 1) % context.numberOfPoints];
                float hypOtherB = context.hypsForward[(i > 0) ? i - 1 : context.numberOfPoints - 1];

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

        private static void CalculateAngles(SkeletonContext context, int pointIndex)
        {

            int ia = (pointIndex > 0) ? pointIndex - 1 : context.numberOfPoints - 1;
            int ib = pointIndex % context.numberOfPoints;
            int ic = (pointIndex + 1) % context.numberOfPoints;

            Vector2 a = context.usePoints[ia];
            Vector2 b = context.usePoints[ib];
            Vector2 c = context.usePoints[ic];

            Vector2 dirA = a - b;
            Vector2 dirB = c - b;

            float tarad = Vector2.Angle(Vector2.up, dirA);
            if (tarad < 0) tarad += 360;
            tarad = tarad * Mathf.Deg2Rad * Mathf.Sign(Vector2.Dot(Vector2.right, dirA));
           
            Vector2 aDir90 = new Vector2(Mathf.Sin(tarad + Mathf.PI / 2), Mathf.Cos(tarad + Mathf.PI / 2));//for use to determine reflex angle using Dot

            float tbrad = Vector2.Angle(Vector2.up, dirB);
            if (tbrad < 0) tbrad += 360;
            tbrad = tbrad * Mathf.Deg2Rad * Mathf.Sign(Vector2.Dot(Vector2.right, dirB));
            Vector2 bDir = new Vector2(Mathf.Sin(tbrad), Mathf.Cos(tbrad));

            float reflex = Vector2.Dot(aDir90, bDir) > 0 ? 0 : -1;
            float drad = (Mathf.LerpAngle(tarad * Mathf.Rad2Deg, tbrad * Mathf.Rad2Deg, 0.5f) + (reflex * 180)) * (Mathf.Deg2Rad);

            context.pointAngles[ib] = drad;
        }

        private static void CalculateHypotenuse(SkeletonContext context, int pointIndex)
        {
            //        bool drawLine = numberOfPoints == 4;
            int pointIndexB = (pointIndex + 1) % context.numberOfPoints;
            Vector2 pa = context.usePoints[pointIndex];
            Vector2 pb = context.usePoints[pointIndexB];
            Vector2 baseDir = pa - pb;
            float baseLength = Vector2.Distance(pa, pb);
            float aa = context.pointAngles[pointIndex];
            float ab = context.pointAngles[pointIndexB];

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

            float hypA = adjactentLength / Mathf.Sin(relAngA);//trig get the hypot
            float hypB = adjactentLength / Mathf.Sin(relAngB);

            //Fast line intersection HERE
            int numberOfOriginalPoints = context.originalPoints.Length;
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
                if (i == ((pointIndex > 0) ? pointIndex - 1 : context.numberOfPoints - 1))
                    skipForard = true;
                if (i == (pointIndex + 1) % context.numberOfPoints)
                    skipBackward = true;
                Vector2 pA0 = context.originalPoints[i];
                Vector2 pA1 = context.originalPoints[(i + 1) % numberOfOriginalPoints];

                if (pA0 == pB0A || pA1 == pB0A)
                    skipForard = true;
                if (pA0 == pB0B || pA1 == pB0B)
                    skipBackward = true;

                if (calculateForward && !skipForard)
                {
                    if (FastLineIntersection(pA0, pA1, pB0A, pB1ha))
                    {
                        //hypontenuse intersects building plan
                        context.hypsForward[pointIndex] = float.PositiveInfinity;
                        calculateForward = false;
                    }
                }
                if (calculateBackward && !skipBackward)
                {
                    if (FastLineIntersection(pA0, pA1, pB0B, pB1hb))
                    {
                        //hypontenuse intersects building plan
                        context.hypsBackward[pointIndexB] = float.PositiveInfinity;
                        calculateBackward = false;
                    }
                }
            }
            if (calculateForward)
            {
                context.hypsForward[pointIndex] = hypA;

            }
            if (calculateBackward)
            {
                context.hypsBackward[pointIndexB] = hypB;
            }
        }

        public static bool FastLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            return (CCW(a1, b1, b2) != CCW(a2, b1, b2)) && (CCW(a1, a2, b1) != CCW(a1, a2, b2));
        }

        private static bool CCW(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return ((p2.x - p1.x) * (p3.y - p1.y) > (p2.y - p1.y) * (p3.x - p1.x));
        }

        public class SkeletonContext
        {
            public int numberOfPoints;
            /// <summary>
            /// return as a set of Vector2 in triangle sets of three
            /// </summary>
            public List<Vector2> returnPoints = new List<Vector2>();
            public Vector2[] originalPoints;
            public List<Vector2> usePoints = new List<Vector2>();

            /// <summary>
            /// points that make up the ridge
            /// </summary>
            public List<Vector2> interiorPoints = new List<Vector2>();
            public List<float> pointAngles = new List<float>();
            /// <summary>
            /// list of hypontenuse for ordering the points
            /// </summary>
            public List<float> hypsForward = new List<float>();
            /// <summary>
            /// list of hypontenuse for ordering the points
            /// </summary>
            public List<float> hypsBackward = new List<float>();
        }
    }
}
