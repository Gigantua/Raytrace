using CPU3D.RayTrace;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Objects
{
    public class Triangle : Object3D
    {
        readonly Vector3[] vertices;
        readonly int ix;
        readonly int iy;
        readonly int iz;

        public Vector3 A => vertices[ix];
        public Vector3 B => vertices[iy];
        public Vector3 C => vertices[iz];

        public Triangle(Vector3[] vertices, Vector3 face)
        {
            this.vertices = vertices;
            ix = (int)face.X;
            iy = (int)face.Y;
            iz = (int)face.Z;
        }

        public Triangle(Vector3 A,Vector3 B, Vector3 C)
        {
            vertices = new Vector3[] { A, B, C };
            ix = 0;
            iy = 1;
            iz = 2;
        }

        public override HitInfo Hittest(Ray r)
        {
            bool hit = HitTest.RayIntersectsTriangle(r, this, out float dist);
            if (!hit) return null;

            return new HitInfo(this, r.Walk(dist), Vector3.UnitY, dist, false);
        }
    }
}
