using Engine2.Raytrace;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Primitives
{
    public class Triangle
    {
        public readonly Vector3[] Elements;
        public Vector3 A => Elements[0];
        public Vector3 B => Elements[1];
        public Vector3 C => Elements[2];

        public readonly Vertex[] Vertices;

        public Vertex TA => Vertices[0];
        public Vertex TB => Vertices[1];
        public Vertex TC => Vertices[2];

        public Vector3 Normal
        {
            get
            {
                var dir = Vector3.Cross(B - A, C - A);
                var norm = Vector3.Normalize(dir);
                return norm;
            }
        }


        public Ray BounceRay(Vector3 HitPosition, Ray r)
        {
            Vector3 UVW = this.Barycentric(HitPosition);
            Vector3 BounceDirection = TA.Normal * UVW[0] + TB.Normal * UVW[1] + TC.Normal * UVW[2];

            Plane P = new Plane(HitPosition, BounceDirection);

            Vector3 dir = Vector3.Transform(r.Direction, P.Reflection3x3());
            return new Ray(r.Position + dir * 0.1f, dir);
        }

        //Wrong because refraction is not negative reflection
        public Ray RefractRay(Vector3 HitPosition, Ray r)
        {
            Vector3 UVW = this.Barycentric(HitPosition);
            Vector3 BounceDirection = TA.Normal * UVW[0] + TB.Normal * UVW[1] + TC.Normal * UVW[2];

            Plane P = new Plane(HitPosition, BounceDirection);

            Vector3 dir = -Vector3.Transform(r.Direction, P.Reflection3x3());
            return new Ray(r.Position + dir * 0.1f, dir);
        }

        public Vector3 Barycentric(Vector3 faceposition)
        {
            Vector3 v0 = B - A, v1 = C - A, v2 = faceposition - A;
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;

            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }

        public void Transform(Matrix M)
        {
            Vertices[0].WorldCoords = Vector3.TransformCoordinate(Vertices[0].Coordinates, M);
            Vertices[1].WorldCoords = Vector3.TransformCoordinate(Vertices[1].Coordinates, M);
            Vertices[2].WorldCoords = Vector3.TransformCoordinate(Vertices[2].Coordinates, M);
        }

        public bool IsIntersected(BoundingBox box)
        {
            Vector3 center = (box.Maximum - box.Minimum) / 2;
            float dX = box.Maximum.X - box.Minimum.X;
            float dY = box.Maximum.Y - box.Minimum.Y;
            float dZ = box.Maximum.Z - box.Minimum.Z;

            return AABBTest.IsIntersecting(box, this);
        }

        public Triangle(Vertex A,Vertex B, Vertex C)
        {
            Elements = new Vector3[] { A.Coordinates, B.Coordinates, C.Coordinates };
            Vertices = new Vertex[] { A, B, C };
        }

        public Triangle(Vector3 A, Vector3 B, Vector3 C)
        {
            Elements = new Vector3[] { A, B, C };
        }

        public bool IsIntersectedByRay(Ray ray)
        {
            return ray.Intersects(ref Elements[0], ref Elements[1], ref Elements[2]);
        }

        public (bool, float) RayIntersection(Ray ray)
        {
            float dist;
            if (ray.Intersects(ref Elements[0], ref Elements[1], ref Elements[2], out dist))
            {
                return (true, dist);
            }
            return (false, 0);
        }
    }

}
