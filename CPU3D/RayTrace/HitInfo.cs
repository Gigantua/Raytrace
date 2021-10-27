using CPU3D.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CPU3D.RayTrace
{
    public class HitInfo
    {
        public Object3D HitObj { get; }
        public Vector3 Normal { get; }
        public float dist { get; }
        public bool InsideObject { get; }
        public Vector3 HitPos { get; }

        public HitInfo(Object3D hitObj, Vector3 HitPos, Vector3 Normal, float dist, bool Inside)
        {
            this.HitObj = hitObj;
            this.dist = dist;
            this.InsideObject = Inside;
            this.HitPos = HitPos;
            this.Normal = Normal;
        }

        /// <summary>
        /// Used for simple hit
        /// </summary>
        public HitInfo(Ray ray)
        {
            HitPos = ray.Direction;
        }
    }
}
