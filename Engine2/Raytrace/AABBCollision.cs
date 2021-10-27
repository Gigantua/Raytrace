using Engine2.Primitives;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Raytrace
{
    static class AABBTest
    {
        public static Vector3[] BoxEdges(BoundingBox box)
        {
            Vector3 A = box.Minimum;
            Vector3 H = box.Maximum;

            float dX = H.X - A.X;
            float dY = H.Y - A.Y;
            float dZ = H.Z - A.Z;

            Vector3 B = A + new Vector3(dX, 0, 0);
            Vector3 C = A + new Vector3(0, 0, dZ);
            Vector3 D = A + new Vector3(dX, 0, dZ);

            Vector3 E = A + new Vector3(0, dY, 0);
            Vector3 F = A + new Vector3(dX, dY, 0);
            Vector3 G = A + new Vector3(0, dY, dZ);

            return new Vector3[] { A, B, C, D, E, F, G ,H};
        }

        public static bool IntersectsBox(Vector3 a, Vector3 b, Vector3 c, Vector3 boxCenter, Vector3 boxExtents)
        {
            // Translate triangle as conceptually moving AABB to origin
            var v0 = (a - boxCenter);
            var v1 = (b - boxCenter);
            var v2 = (c - boxCenter);

            // Compute edge vectors for triangle
            var f0 = (v1 - v0);
            var f1 = (v2 - v1);
            var f2 = (v0 - v2);

            #region Test axes a00..a22 (category 3)

            // Test axis a00
            var a00 = new Vector3(0, -f0.Z, f0.Y);
            var p0 = Vector3.Dot(v0, a00);
            var p1 = Vector3.Dot(v1, a00);
            var p2 = Vector3.Dot(v2, a00);
            var r = boxExtents.Y * Math.Abs(f0.Z) + boxExtents.Z * Math.Abs(f0.Y);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a01
            var a01 = new Vector3(0, -f1.Z, f1.Y);
            p0 = Vector3.Dot(v0, a01);
            p1 = Vector3.Dot(v1, a01);
            p2 = Vector3.Dot(v2, a01);
            r = boxExtents.Y * Math.Abs(f1.Z) + boxExtents.Z * Math.Abs(f1.Y);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a02
            var a02 = new Vector3(0, -f2.Z, f2.Y);
            p0 = Vector3.Dot(v0, a02);
            p1 = Vector3.Dot(v1, a02);
            p2 = Vector3.Dot(v2, a02);
            r = boxExtents.Y * Math.Abs(f2.Z) + boxExtents.Z * Math.Abs(f2.Y);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a10
            var a10 = new Vector3(f0.Z, 0, -f0.X);
            p0 = Vector3.Dot(v0, a10);
            p1 = Vector3.Dot(v1, a10);
            p2 = Vector3.Dot(v2, a10);
            r = boxExtents.X * Math.Abs(f0.Z) + boxExtents.Z * Math.Abs(f0.X);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a11
            var a11 = new Vector3(f1.Z, 0, -f1.X);
            p0 = Vector3.Dot(v0, a11);
            p1 = Vector3.Dot(v1, a11);
            p2 = Vector3.Dot(v2, a11);
            r = boxExtents.X * Math.Abs(f1.Z) + boxExtents.Z * Math.Abs(f1.X);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a12
            var a12 = new Vector3(f2.Z, 0, -f2.X);
            p0 = Vector3.Dot(v0, a12);
            p1 = Vector3.Dot(v1, a12);
            p2 = Vector3.Dot(v2, a12);
            r = boxExtents.X * Math.Abs(f2.Z) + boxExtents.Z * Math.Abs(f2.X);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a20
            var a20 = new Vector3(-f0.Y, f0.X, 0);
            p0 = Vector3.Dot(v0, a20);
            p1 = Vector3.Dot(v1, a20);
            p2 = Vector3.Dot(v2, a20);
            r = boxExtents.X * Math.Abs(f0.Y) + boxExtents.Y * Math.Abs(f0.X);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a21
            var a21 = new Vector3(-f1.Y, f1.X, 0);
            p0 = Vector3.Dot(v0, a21);
            p1 = Vector3.Dot(v1, a21);
            p2 = Vector3.Dot(v2, a21);
            r = boxExtents.X * Math.Abs(f1.Y) + boxExtents.Y * Math.Abs(f1.X);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            // Test axis a22
            var a22 = new Vector3(-f2.Y, f2.X, 0);
            p0 = Vector3.Dot(v0, a22);
            p1 = Vector3.Dot(v1, a22);
            p2 = Vector3.Dot(v2, a22);
            r = boxExtents.X * Math.Abs(f2.Y) + boxExtents.Y * Math.Abs(f2.X);
            if (Math.Max(-fmax(p0, p1, p2), fmin(p0, p1, p2)) > r)
            {
                return false;
            }

            #endregion

            #region Test the three axes corresponding to the face normals of AABB b (category 1)

            // Exit if...
            // ... [-extents.X, extents.X] and [min(v0.X,v1.X,v2.X), max(v0.X,v1.X,v2.X)] do not overlap
            if (fmax(v0.X, v1.X, v2.X) < -boxExtents.X || fmin(v0.X, v1.X, v2.X) > boxExtents.X)
            {
                return false;
            }

            // ... [-extents.Y, extents.Y] and [min(v0.Y,v1.Y,v2.Y), max(v0.Y,v1.Y,v2.Y)] do not overlap
            if (fmax(v0.Y, v1.Y, v2.Y) < -boxExtents.Y || fmin(v0.Y, v1.Y, v2.Y) > boxExtents.Y)
            {
                return false;
            }

            // ... [-extents.Z, extents.Z] and [min(v0.Z,v1.Z,v2.Z), max(v0.Z,v1.Z,v2.Z)] do not overlap
            if (fmax(v0.Z, v1.Z, v2.Z) < -boxExtents.Z || fmin(v0.Z, v1.Z, v2.Z) > boxExtents.Z)
            {
                return false;
            }

            #endregion

            #region Test separating axis corresponding to triangle face normal (category 2)

            var planeNormal = Vector3.Cross(f0, f1);
            var planeDistance = Vector3.Dot(planeNormal, v0);

            // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
            r = boxExtents.X * Math.Abs(planeNormal.X)
                + boxExtents.Y * Math.Abs(planeNormal.Y)
                + boxExtents.Z * Math.Abs(planeNormal.Z);

            // Intersection occurs when plane distance falls within [-r,+r] interval
            if (planeDistance > r)
            {
                return false;
            }

            #endregion

            return true;
        }

        private static float fmin(float a, float b, float c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        private static float fmax(float a, float b, float c)
        {
            return Math.Max(a, Math.Max(b, c));
        }


        public static bool IsIntersecting(BoundingBox box, Triangle triangle)
        {
            double triangleMin, triangleMax;
            double boxMin, boxMax;

            // Test the box normals (x-, y- and z-axes)
            var boxNormals = new Vector3[] {
                new Vector3(1,0,0),
                new Vector3(0,1,0),
                new Vector3(0,0,1)
            };
            for (int i = 0; i < 3; i++)
            {
                Vector3 n = boxNormals[i];
                Project(triangle.Elements, boxNormals[i], out triangleMin, out triangleMax);
                if (triangleMax < box.Minimum[i] || triangleMin > box.Maximum[i])
                    return false; // No intersection possible.
            }

            // Test the triangle normal
            double triangleOffset = Vector3.Dot(triangle.Normal, triangle.A);
            Project(BoxEdges(box), triangle.Normal, out boxMin, out boxMax);
            if (boxMax < triangleOffset || boxMin > triangleOffset)
                return false; // No intersection possible.

            // Test the nine edge cross-products
            Vector3[] triangleEdges = new Vector3[] {
                triangle.A - triangle.B,
                triangle.B - triangle.C,
                triangle.C - triangle.A
            };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    // The box normals are the same as it's edge tangents
                    Vector3 axis = Vector3.Cross(triangleEdges[i],boxNormals[j]);
                    Project(BoxEdges(box), axis, out boxMin, out boxMax);
                    Project(triangle.Elements, axis, out triangleMin, out triangleMax);
                    if (boxMax <= triangleMin || boxMin >= triangleMax)
                        return false; // No intersection possible
                }

            // No separating axis found.
            return true;
        }

        static void Project(Vector3[] points, Vector3 axis, out double min, out double max)
        {
            min = double.PositiveInfinity;
            max = double.NegativeInfinity;
            foreach (var p in points)
            {
                double val = Vector3.Dot(axis, p);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        interface IVector
        {
            double X { get; }
            double Y { get; }
            double Z { get; }
            double[] Coords { get; }
            double Dot(IVector other);
            IVector Minus(IVector other);
            IVector Cross(IVector other);
        }

        

        interface IShape
        {
            IEnumerable<IVector> Vertices { get; }
        }

        interface ITriangle : IShape
        {
            IVector Normal { get; }
            IVector A { get; }
            IVector B { get; }
            IVector C { get; }
        }
    }
    
}
