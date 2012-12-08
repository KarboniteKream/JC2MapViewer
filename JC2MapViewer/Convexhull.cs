using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace JC2MapViewer
{
    public class Convexhull
    {
        // given a polygon formed by pts, return the subset of those points  
        // that form the convex hull of the polygon  

        public static void MinPoints(ref Point[] pts, ref Point origin)
        {
            double minx = double.MaxValue;
            double miny = double.MaxValue;
            for (int i = 0; i < pts.Length; i++)
            {
                minx = Math.Min(pts[i].X, minx);
                miny = Math.Min(pts[i].Y, miny);
            }

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = new Point(pts[i].X - minx, pts[i].Y - miny);
            }
            origin = new Point(minx, miny);
        }
        
        public static Point[] ConvexHull(Point[] pts)
        {
            Point[][] l_u = ConvexHull_LU(pts);
            Point[] lower = l_u[0];
            Point[] upper = l_u[1];
            // Join the lower and upper hull  
            int nl = lower.Length;
            int nu = upper.Length;
            Point[] result = new Point[nl + nu];
            for (int i = 0; i < nl; i++)
                result[i] = lower[i];
            for (int i = 0; i < nu; i++)
                result[i + nl] = upper[i];
            return result;
        }

        // returns the two points that form the diameter of the polygon formed by points pts  
        public static Point[] Diameter(Point[] pts)
        {
            IEnumerable<Pair> pairs = RotatingCalipers(pts);
            double max2 = Double.NegativeInfinity;
            Pair maxPair = null;
            foreach (Pair pair in pairs)
            {
                Point p = pair.a;
                Point q = pair.b;
                double dx = p.X - q.X;
                double dy = p.Y - q.Y;
                double dist2 = dx * dx + dy * dy;
                if (dist2 > max2)
                {
                    maxPair = pair;
                    max2 = dist2;
                }
            }

            // return Math.Sqrt(max2);  
            return new Point[] { maxPair.a, maxPair.b };
        }

        private static double Orientation(Point p, Point q, Point r)
        {
            return (q.Y - p.Y) * (r.X - p.X) - (q.X - p.X) * (r.Y - p.Y);
        }

        private static void Pop<T>(List<T> l)
        {
            int n = l.Count;
            l.RemoveAt(n - 1);
        }

        private static T At<T>(List<T> l, int index)
        {
            int n = l.Count;
            if (index < 0)
                return l[n + index];
            return l[index];
        }

        private static Point[][] ConvexHull_LU(Point[] arr_pts)
        {
            List<Point> u = new List<Point>();
            List<Point> l = new List<Point>();
            List<Point> pts = new List<Point>(arr_pts.Length);
            pts.AddRange(arr_pts);
            pts.Sort(Compare);
            foreach (Point p in pts)
            {
                while (u.Count > 1 && Orientation(At(u, -2), At(u, -1), p) <= 0) Pop(u);
                while (l.Count > 1 && Orientation(At(l, -2), At(l, -1), p) >= 0) Pop(l);
                u.Add(p);
                l.Add(p);
            }
            return new Point[][] { l.ToArray(), u.ToArray() };
        }

        private class Pair
        {
            public Point a, b;
            public Pair(Point a, Point b)
            {
                this.a = a;
                this.b = b;
            }
        }

        private static IEnumerable<Pair> RotatingCalipers(Point[] pts)
        {
            Point[][] l_u = ConvexHull_LU(pts);
            Point[] lower = l_u[0];
            Point[] upper = l_u[1];
            int i = 0;
            int j = lower.Length - 1;
            while (i < upper.Length - 1 || j > 0)
            {
                yield return new Pair(upper[i], lower[j]);
                if (i == upper.Length - 1) j--;
                else if (j == 0) i += 1;
                else if ((upper[i + 1].Y - upper[i].Y) * (lower[j].X - lower[j - 1].X) >
                    (lower[j].Y - lower[j - 1].Y) * (upper[i + 1].X - upper[i].X))
                    i++;
                else
                    j--;
            }
        }

        private static int Compare(Point a, Point b)
        {
            if (a.X < b.X)
            {
                return -1;
            }
            else if (a.X == b.X)
            {
                if (a.Y < b.Y)
                    return -1;
                else if (a.Y == b.Y)
                    return 0;
            }
            return 1;
        }
    }
}