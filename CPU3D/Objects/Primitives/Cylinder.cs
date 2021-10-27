using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class Cylinder : Object3D
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 AUp;
        public Vector3 BDown;
        
        public Vector3 AB;
        
        public float Radius;
        public float RSquared;

        public Cylinder(Vector3 BottomCenter,Vector3 TopCenter, float Radius)
        {
            Vector3 dir = Vector3.Normalize(TopCenter - BottomCenter);
            this.B = Vector3.Max(TopCenter, BottomCenter);
            this.A = Vector3.Min(TopCenter, BottomCenter);

            this.AUp = Vector3.Normalize(B - A);
            this.BDown = -AUp;
            
            this.Radius = Radius;
            this.RSquared = Radius * Radius;
            this.AB = B - A;

            Vector3 left = Vector3.Normalize(Vector3.Cross(dir + new Vector3(1.2f, 0.2f, 1.4f), dir));
            Vector3 right = Vector3.Normalize(Vector3.Cross(left, dir));

            base.HitBox = new AABB(A + left * Radius * 1.5f, B + right * Radius * 1.5f);
        }

        public override HitInfo Hittest(Ray r)
        {
            return HitTest.RayIntersectsCylinder(r, this);
        }
    }
}
