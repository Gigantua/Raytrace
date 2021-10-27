using CPU3D.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.RayTrace
{
    public class FresnelEquation
    {
        static readonly float Epsilon = 0.008f;

        readonly float n1;
        readonly float n2;
        readonly Vector3 SurfacePos;
        readonly Vector3 Normal;
        readonly Vector3 reflectdir;
        readonly Vector3 refractdir;

        //https://graphics.stanford.edu/courses/cs148-10-summer/docs/2006--degreve--reflection_refraction.pdf


        public FresnelEquation(Ray ray, float RefractionIndexObject, Vector3 Normal, Vector3 SurfacePosition)
        {
            this.SurfacePos = SurfacePosition;
            this.Normal = Normal;
            this.n1 = ray.RefractiveIndex;
            this.n2 = RefractionIndexObject;
            float n = n1 / n2;

            float CosAlpha = -Vector3.Dot(Normal, ray.Direction);

            float det = n * n * (1 - CosAlpha * CosAlpha);
            if (det <= 1.0f) //Normal reflection
            {
                float CosBeta = FMath.Sqrt(1 - det);
                this.R = Reflectiveness(CosAlpha, CosBeta);
                this.T = 1 - R;
                this.reflectdir = ray.Direction + 2 * CosAlpha * Normal;
                this.refractdir = n * ray.Direction + (n * CosAlpha - CosBeta) * Normal;
            }
            else //total internal reflection
            {
                this.R = 1;
                this.T = 0;
                this.reflectdir = ray.Direction + 2 * CosAlpha * Normal;
            }
        }

        public static Ray ReflectSurface(Ray ray, Vector3 HitPos, Vector3 Normal)
        {
            float CosAlpha = -Vector3.Dot(Normal, ray.Direction);
            Vector3 reflectdir = ray.Direction + 2 * CosAlpha * Normal;

            return new Ray(HitPos, reflectdir, Epsilon, ray.RefractiveIndex);
        }

        public Ray ReflectRay => new Ray(SurfacePos, reflectdir, Epsilon, n1); //Keep refractive Index
        public Ray RefractRay => new Ray(SurfacePos, refractdir, Epsilon, n2); //Change refractive Index

        public float R { get; }
        public float T { get; }

        //25.9ms No Optimisation
        //25.3ms Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Reflectiveness(float CosAlpha, float CosBeta) //Schlick aproximation is slower than this.
        {
            Vector2 CosAB = new Vector2(CosAlpha, CosBeta);
            Vector2 CosBA = new Vector2(CosBeta, CosAlpha);

            Vector2 N1A = n1 * CosAB;
            Vector2 N2B = n2 * CosBA;
            
            Vector2 Rsp = (N1A - N2B) / (N1A + N2B);
            return 0.5f * Rsp.LengthSquared();
            /*
            float Rs = (n1 * CosAlpha - n2 * CosBeta) / (n1 * CosAlpha + n2 * CosBeta); //sqrt of s polarized light
            float Rp = (n1 * CosBeta - n2 * CosAlpha) / (n1 * CosBeta + n2 * CosAlpha); //sqrt of p polarized light
            
            float R = 0.5f * (Rs * Rs + Rp * Rp);
            return R;
            */
        }


    }


}
