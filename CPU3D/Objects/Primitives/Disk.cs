using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class Disk : Object3D
    {
        public Vector3 Center;
        public Vector3 Normal;
        public float RIn2;
        public float ROut2;

        public Disk(Vector3 Position, Vector3 Normal, float InnerRadius, float OuterRadius)
        {
            this.Center = Position;
            this.Normal = Normal;
            this.RIn2 = InnerRadius * InnerRadius;
            this.ROut2 = OuterRadius * OuterRadius;

            Vector3 left = Vector3.Normalize(Vector3.Cross(Normal + new Vector3(1.2f, 0.2f, 1.4f), Normal));
            Vector3 right = Vector3.Normalize(Vector3.Cross(left, Normal));

            base.HitBox = new AABB(Center - Normal + left * OuterRadius * 1.5f, Center + Normal + right * OuterRadius * 1.5f);
        }

        public static Plane XYPlane => new Plane(0, Vector3.UnitZ);
        public static Plane XZPlane => new Plane(0, Vector3.UnitY);
        public static Plane YZPlane => new Plane(0, Vector3.UnitX);

        public override HitInfo Hittest(Ray r)
        {
            return HitTest.RayIntersectsDisk(r, this);
        }
    }
}
