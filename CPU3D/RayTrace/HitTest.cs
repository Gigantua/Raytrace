using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CPU3D.Objects;

namespace CPU3D.RayTrace
{
    public static partial class HitTest
    {
        public static readonly float Epsilon = 0.0000001f;
        public static readonly float EpsilonInv = 1 / Epsilon;

        public static bool RayIntersectsTriangle(Ray ray, Triangle face, out float dist)
        {
            dist = 0;
            Vector3 vertex0 = face.A;
            Vector3 vertex1 = face.B;
            Vector3 vertex2 = face.C;
            Vector3 edge1, edge2, h, s, q;
            float a, f, u, v;
            edge1 = vertex1 - vertex0;
            edge2 = vertex2 - vertex0;

            h = Vector3.Cross(ray.Direction,edge2);
            a = Vector3.Dot(edge1, h);
            if (a > -Epsilon && a < Epsilon)
                return false;
            f = 1 / a;
            s = ray.Origin - vertex0;
            u = f * (Vector3.Dot(s, h));
            if (u < 0.0 || u > 1.0)
                return false;

            q = Vector3.Cross(s, edge1);
            v = f * Vector3.Dot(ray.Direction, q);
            if (v < 0.0 || u + v > 1.0)
                return false;
            // At this stage we can compute t to find out where the intersection point is on the line.
            float t = f * Vector3.Dot(edge2, q);
            if (t > Epsilon) //ray intersection
            {
                dist = t;
                return true;
            }
            else // This means that there is a line intersection but not a ray intersection.
                return false;
        }

        public static bool RayIntersectsPlane(Ray r, Objects.Plane p, out float dist, out Vector3 Normal) 
        {
            dist = 0;
            Normal = p.Normal;
            float denom = Vector3.Dot(p.Normal, r.Direction);
            if (Math.Abs(denom) > Epsilon)
            {
                dist = Vector3.Dot(p.Center - r.Origin, p.Normal) / denom;
                if (dist >= 0) return true;
            }
            /*
            if (!AbovePlane(r.Origin,r.Walk(dist),Normal))
            {
                Normal = -p.Normal;
            }
            */
            return false;
        } 

        public static HitInfo RayIntersectsDisk(Ray r, Objects.Disk d)
        {
            float denom = Vector3.Dot(d.Normal, r.Direction);

            float dist = Vector3.Dot(d.Center - r.Origin, d.Normal) / denom;
            if (dist < 0) return null;

            Vector3 HitPos = r.Walk(dist);
            float R2 = Vector3.DistanceSquared(HitPos, d.Center);

            if (R2 > d.ROut2 || R2 < d.RIn2) return null;

            return new HitInfo(d, HitPos, d.Normal, dist, false);
        }

        static float CylinderCap(Ray r, float R2, Vector3 CapCenter, Vector3 CapNormal, ref Vector3 HitPos)
        {
            float denom = Vector3.Dot(CapNormal, r.Direction);
            if (Math.Abs(denom) < Epsilon) return float.MaxValue;

            float dist = Vector3.Dot(CapCenter - r.Origin, CapNormal) / denom;
            if (dist < 0) return float.MaxValue;

            HitPos = r.Walk(dist);
            float CenterDist = Vector3.DistanceSquared(HitPos, CapCenter);
            if (CenterDist > R2) return float.MaxValue;

            return dist;
        }

        public static HitInfo RayIntersectsCylinder(Ray r, Objects.Cylinder cyl)
        {
            Vector3 HitPos = new Vector3(0,0,0);

            Vector3 AB = cyl.AB;
            Vector3 AO = r.Origin - cyl.A;
            Vector3 BO = r.Origin - cyl.B;
            float R2 = cyl.RSquared;

            bool above = Vector3.Dot(BO, cyl.AUp) > 0;
            bool below = Vector3.Dot(AO, cyl.BDown) > 0;

            bool Inside = false;
            if (!above && !below) //Inside Planes so we check distance from axis
            {
                float Linedist2 = Vector3.Cross(AO, BO).LengthSquared() / (cyl.AB.LengthSquared());
                Inside = Linedist2 < R2;
            }

            if (above) //Check top first
            {
                float d = CylinderCap(r, R2, cyl.B, cyl.BDown, ref HitPos);
                if (d != float.MaxValue) return new HitInfo(cyl, HitPos, cyl.AUp, d, Inside);
            }
            else if (below) //Check bottom first
            {
                float d = CylinderCap(r, R2, cyl.A, cyl.AUp, ref HitPos);
                if (d != float.MaxValue) return new HitInfo(cyl, HitPos, cyl.BDown, d, Inside);
            }

            float AxisAlignment = Vector3.Dot(AB, r.Direction);
            float AxisOffset = Vector3.Dot(AB, AO);
            float h2 = AB.LengthSquared();

            float m = AxisAlignment / h2;
            float n = AxisOffset / h2;

            Vector3 Q = r.Direction - (AB * m);
            Vector3 R = AO - (AB * n);

            float a = 2 * Q.LengthSquared();
            float b = Vector3.Dot(Q, R) * 2;
            float c = R.LengthSquared() - R2;

            float discriminant = b * b - 2 * a * c;
            if (discriminant < 0) return null; //Ray missed cylinder

            float tb = FMath.Sqrt(discriminant);

            float tmax = (-b + tb) / a;
            if (tmax < 0) return null; //Cylinder behind us
            float tmin = (-b - tb) / a;
            tmin = tmin < 0 ? tmax : tmin; 

            float m1 = tmin * m + n; //m1 is percentage of height which is hit
            if (m1 > 0 && m1 < 1)
            {
                HitPos = r.Walk(tmin);
                Vector3 k1 = cyl.A + AB * m1;
                Vector3 Normal = Vector3.Normalize(HitPos - k1);
                if (Inside) Normal = -Normal;

                return new HitInfo(cyl, HitPos, Normal, tmin, Inside);
            }
            if (Inside) //We are inside and did not hit mantle so we can still hit caps
            {
                if (m1 > 1)
                {
                    float d = CylinderCap(r, R2, cyl.B, cyl.BDown, ref HitPos);
                    return new HitInfo(cyl, HitPos, cyl.BDown, d, Inside);
                }
                else
                {
                    float d = CylinderCap(r, R2, cyl.A, cyl.AUp, ref HitPos);
                    return new HitInfo(cyl, HitPos, cyl.AUp, d, Inside);
                }
            }

            return null;
        }


        public static HitInfo RayIntersectsSphere(Ray r, Objects.Sphere s)
        {
            Vector3 L = s.Position - r.Origin;
            float tA = Vector3.Dot(L, r.Direction);

            float dR = s.RSquared - L.LengthSquared();

            float discr = dR + tA * tA;
            if (discr < 0) return null;

            float tB = FMath.Sqrt(discr);

            float dist;
            bool Inside = dR > 0;
            if (Inside) dist = tA + tB;
            else
            {
                dist = tA - tB;
                if (dist < 0) return null;
            }

            Vector3 hitp = r.Walk(dist);
            Vector3 Normal = Inside ? Vector3.Normalize(s.Position - hitp) : Vector3.Normalize(hitp - s.Position);

            return new HitInfo(s, hitp, Normal, dist, Inside);
        }

        public static bool RayIntersectsQuad(Quad q, Ray r, out float dist)
        {
            dist = 0;
            Vector3 min = q.A;
            Vector3 max = q.B;

            Vector3 Tmin = (min - r.Origin) * r.InverseDirection;
            Vector3 Tmax = (max - r.Origin) * r.InverseDirection;

            float tmin = Math.Max(Math.Max(Tmin.X, Tmin.Y), Tmin.Z);
            float tmax = Math.Min(Math.Min(Tmax.X, Tmax.Y), Tmax.Z);
            if (tmax < 0) return false;
            if (tmin > tmax) return false;

            dist = tmin;
            return true;
        }



        public static float RayIntersectsBox(Box box, Ray r, out bool Inside)
        {
            Inside = false;

            Vector3 td1 = (box.min - r.Origin) * r.InverseDirection;
            Vector3 td2 = (box.max - r.Origin) * r.InverseDirection;

            Vector3 tdmax = Vector3.Max(td1, td2);
            float tmax = tdmax.Min();
            if (tmax < 0) return float.MaxValue;

            Vector3 tdmin = Vector3.Min(td1, td2);
            float tmin = tdmin.Max();
            if (tmax < tmin) return float.MaxValue;

            Inside = tmin < 0;
            return Inside ? tmax : tmin;
        }

        /// <summary>
        /// Returns distance to an AABB. Inside = 0, Hit = dist, No Hit = float.PositiveInfinity
        /// </summary>
        public static float RayIntersectsAABB(Box box, Ray r)
        {
            Vector3 td1 = (box.min - r.Origin) * r.InverseDirection;
            Vector3 td2 = (box.max - r.Origin) * r.InverseDirection;

            Vector3 tdmax = Vector3.Max(td1, td2);
            float tmax = tdmax.Min();
            if (tmax < 0) return float.PositiveInfinity; //No intersection

            Vector3 tdmin = Vector3.Min(td1, td2);
            float tmin = tdmin.Max();
            if (tmax < tmin) return float.PositiveInfinity; //Behind ray
            if (tmin < 0) return 0; //Inside Box (if you want real dist if inside call RayIntersetsBox)

            return tmax; 
        }

        static float squared(float v) { return v * v; }
        public static bool SphereIntersectsBox(Box box, Sphere s)
        {
            var C1 = box.min;
            var C2 = box.max;
            float dist_squared = s.RSquared;
            var S = s.Position;

            if (S.X < C1.X) dist_squared -= squared(S.X - C1.X);
            else if (S.X > C2.X) dist_squared -= squared(S.X - C2.X);
            if (S.Y < C1.Y) dist_squared -= squared(S.Y - C1.Y);
            else if (S.Y > C2.Y) dist_squared -= squared(S.Y - C2.Y);
            if (S.Z < C1.Z) dist_squared -= squared(S.Z - C1.Z);
            else if (S.Z > C2.Z) dist_squared -= squared(S.Z - C2.Z);
            return dist_squared > 0;
        }

        public static bool BoxIntersectsBox(Box A, Box B)
        {
            if (A.max.X < B.min.X) return false;
            if (A.min.X > B.max.X) return false;

            if (A.max.Y < B.min.Y) return false;
            if (A.min.Y > B.max.Y) return false;

            if (A.max.Z < B.min.Z) return false;
            if (A.min.Z > B.max.Z) return false;

            return true;
        }

        public static bool DiskIntersectsBox(Box box, Disk d)
        {
            Vector3 e = box.max - box.center; // Compute positive extents

            // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
            float r = e.X * Math.Abs(d.Normal.X) + e.Y * Math.Abs(d.Normal.Y) + e.Z * Math.Abs(d.Normal.Z);

            // Compute distance of box center from plane
            float s = Vector3.Dot(d.Normal, box.center) - Vector3.Distance(d.Center, Vector3.Zero);

            //Intersection with plane occurs when distance s falls within [-r,+r] interval
            if (Math.Abs(s) > r) return false; //Plane of disk not intersecting AABB

            Ray ray = new Ray(d.Center, box.center - d.Center);
            Vector3 td1 = (box.min - ray.Origin) * ray.InverseDirection;
            Vector3 td2 = (box.max - ray.Origin) * ray.InverseDirection;

            Vector3 tdmin = Vector3.Min(td1, td2);
            Vector3 tdmax = Vector3.Max(td1, td2);

            float tmax = tdmax.Min();
            float tmin = tdmin.Max();

            //Ray is inside box from s1 to s2
            float s1 = Math.Min(tmax, tmin);
            float s2 = Math.Max(tmax, tmin);

            //Disk is also present from r1 to r2
            float r1 = d.RIn2;
            float r2 = d.ROut2;

            bool overlap = s1 < r2 && r1 < s2;

            return overlap;
        }


    }
}
