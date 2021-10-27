using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace RayForm
{
    public static class Hit
    {
        public static readonly f8 None = f8.Create((float)1E9);
        public static readonly f8 NoneCmp = f8.Create((float)1E6);
    }

    public class Sphere8
    {
        public readonly f8 Radius;
        public readonly f8 RSquared;

        public readonly Vector8 Position;

        public Sphere8(Vector8 Position, f8 Radius)
        {
            this.Position = Position;
            this.Radius = Radius;
            this.RSquared = Radius * Radius;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f8 Intersect(Ray8 r, out Vector8 normal)
        {

            Vector8 L = Position - r.Origin;
            f8 tca = Vector8.Dot(L, r.Direction);
            f8 d2 = Vector8.DotA_SubB(L, tca * tca);

            f8 thc = f8.FastSqrt(RSquared - d2);
            f8 t0 = tca - thc;
            f8 t1 = tca + thc; 

            f8 distance = (t0 > 0).Select(t0, t1);
            fbool nohit = distance < 0;

            normal = r.Walk(distance) - Position;

            return nohit.Select(Hit.None, distance);
        }

    }
}
