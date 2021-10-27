using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace RayForm
{
    public class Plane1
    {
        public Plane1(Vector8 Origin, Vector8 Normal)
        {
            this.Origin = Origin;
            this.Normal = Normal;
        }

        public f8 RadiusSquared { get; set; } = new f8(60);

        public Vector8 Origin { get; }
        public Vector8 Normal { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f8 Intersect(Ray8 rays, out Vector8 hitpoint, out Vector8 normal)
        {
            normal = Normal;
            f8 denom = Vector8.RecDot(Normal, rays.Direction);
            f8 distance = Vector8.Dot((Origin - rays.Origin), Normal) * denom;

            fbool Hitmask = denom > 0;
            Hitmask |= distance < 0;

            hitpoint = rays.Walk(distance);
            f8 dist2 = Vector8.DistanceSquared(Origin, hitpoint);
            Hitmask |= dist2 > RadiusSquared;

            return Hitmask.Select(Hit.None, distance);
        }

    }

    public class Plane8
    {
        public Plane8(Vector8 Origin, Vector8 Normal)
        {
            this.Origin = Origin;
            this.Normal = Normal;
        }

        public f8 RadiusSquared { get; set; } = new f8(60);

        public Vector8 Origin { get; }
        public Vector8 Normal { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f8 Intersect(Ray8 rays, out Vector8 hitpoint, out Vector8 normal)
        {
            normal = Normal;
            f8 denom = Vector8.RecDot(Normal, rays.Direction);
            f8 distance = Vector8.Dot((Origin - rays.Origin), Normal) * denom;

            fbool Hitmask = denom > 0;
            Hitmask |= distance < 0;

            hitpoint = rays.Walk(distance);
            f8 dist2 = Vector8.DistanceSquared(Origin, hitpoint);
            Hitmask |= dist2 > RadiusSquared;

            return Hitmask.Select(Hit.None, distance);
        }
    }
}
