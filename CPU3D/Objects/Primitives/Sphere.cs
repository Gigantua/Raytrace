using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class Sphere : Object3D
    {
        public readonly float Radius;
        public readonly float RSquared;

        public readonly Vector3 Position;

        public Sphere(Vector3 Position, float Radius)
        {
            this.Position = Position;
            this.Radius = Radius;
            this.RSquared = Radius * Radius;

            base.HitBox = new AABB(Position - Vector3.One * Radius * 1.5f, Position + Vector3.One * Radius * 1.5f);
        }


        public override HitInfo Hittest(Ray r)
        {
            return HitTest.RayIntersectsSphere(r, this);
        }
        
    }
}
