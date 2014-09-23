using System.Collections.Generic;

namespace Mercraft.Core.Algorithms
{
    public class Triangulator
    {
        public static int[] Triangulate(MapPoint[] points, bool reverse = true)
        {
            var indices = new List<int>((points.Length -2) * 3);

            int n = points.Length;
            if (n < 3)
                return indices.ToArray();

            int[] V = new int[n];
            if (Area(points) > 0)
            {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            }
            else
            {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2; )
            {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V, points))
                {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            if(reverse)
                indices.Reverse();

            return indices.ToArray();
        }

        private static float Area(MapPoint[] points)
        {
            int n = points.Length;
            float a = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                MapPoint pval = points[p];
                MapPoint qval = points[q];
                a += pval.X * qval.Y - qval.X * pval.Y;
            }
            return (a * 0.5f);
        }

        private static bool Snip(int u, int v, int w, int n, int[] V, MapPoint[] points)
        {
            int p;
            MapPoint A = points[V[u]];
            MapPoint B = points[V[v]];
            MapPoint C = points[V[w]];
            if (float.Epsilon > (((B.X - A.X) * (C.Y - A.Y)) - ((B.Y - A.Y) * (C.X - A.X))))
                return false;
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                MapPoint P = points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        private static bool InsideTriangle(MapPoint a, MapPoint b, MapPoint c, MapPoint p)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = c.X - b.X;
            ay = c.Y - b.Y;
            bx = a.X - c.X;
            by = a.Y - c.Y;
            cx = b.X - a.X;
            cy = b.Y - a.Y;
            apx = p.X - a.X;
            apy = p.Y - a.Y;
            bpx = p.X - b.X;
            bpy = p.Y - b.Y;
            cpx = p.X - c.X;
            cpy = p.Y - c.Y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }
}