using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class Box : Object3D
    {
        public Vector3 min { get; set; }
        public Vector3 max { get; set; }
        public Vector3 center { get; private set; }

        Vector3 dr;
        Vector3 ndr;

        public Box(Vector3 min, Vector3 max)
        {
            this.min = Vector3.Min(min, max);
            this.max = Vector3.Max(min, max);
            Recalculate();
        }

        public void Recalculate()
        {
            this.center = (max + min) / 2;
            this.dr = Vector3.Abs(min - max) / 2.0008f; //This is an implicit bias for cast to int later
            this.ndr = -Vector3.Abs(min - max) / 2.0008f; //This is an implicit bias for cast to int later
        }

        
        Vector3 GetBoxNormal(Vector3 dir, bool Inside)
        {
            if (Inside)
            {
                Vector3 NR = dir / ndr;
                return new Vector3((int)NR.X, (int)NR.Y, (int)NR.Z);
            }
            else
            {
                Vector3 N = dir / dr;
                return new Vector3((int)N.X, (int)N.Y, (int)N.Z);
            }
        }

        public override HitInfo Hittest(Ray r)
        {
            float dist = HitTest.RayIntersectsBox(this, r, out bool Inside);
            if (dist == float.MaxValue) return null;

            Vector3 HitPoint = r.Walk(dist);
            Vector3 Normal = Vector3.Normalize(GetBoxNormal(HitPoint - center, Inside));

            return new HitInfo(this, HitPoint, Normal, dist, Inside);
        }
    }
}
