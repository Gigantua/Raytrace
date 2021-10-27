
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class Quad : Object3D
    {
        public readonly Vector3 Normal;
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly Vector3 C;
        public readonly Vector3 D;

        public Quad(Vector3 A, Vector3 B, Vector3 C)
        {
            this.A = Vector3.Min(Vector3.Min(A, B), C);
            this.B = Vector3.Min(Vector3.Max(A, B), C);
            this.C = Vector3.Max(Vector3.Min(A, B), C);
            this.D = C + (B - A);

            Normal = Vector3.Normalize(Vector3.Cross(this.A, this.B));
        }

        public override HitInfo Hittest(Ray r) => null;
    }
}
