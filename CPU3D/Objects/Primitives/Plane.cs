using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class Plane : Object3D
    {
        public float Distance { get; private set; }
        public Vector3 Normal { get; private set; }
        public Vector3 Center { get; private set; }

        public Plane(float Distance, Vector3 Normal)
        {
            this.Distance = Distance;
            this.Normal = Vector3.Normalize(Normal);
            this.Center = this.Normal * this.Distance;
        }

        public static Plane XYPlane => new Plane(0, Vector3.UnitZ);
        public static Plane XZPlane => new Plane(0, Vector3.UnitY);
        public static Plane YZPlane => new Plane(0, Vector3.UnitX);

        public override HitInfo Hittest(Ray r)
        {
            bool hit = HitTest.RayIntersectsPlane(r, this, out float dist, out Vector3 Normal);
            if (!hit) return null;

            return new HitInfo(this, r.Walk(dist), Normal, dist, false);
        }
    }
}
