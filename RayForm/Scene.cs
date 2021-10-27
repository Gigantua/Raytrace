using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace RayForm
{
    public class Scene
    {
        f8[] Xs;
        public void Prepare(int width, int height)
        {
            Xs = new f8[width / 8];
            for (int x = 0; x < width; x += 8)
            {
                Xs[x/8] = ((new f8(x) + f8.ZeroTo7) / width - 0.5f) * 2;
            }
        }

        static readonly Vector256<int> OneInt = Vector256.Create(1);
        static readonly Vector8 SkyColor = new Vector8(51 / 255.0f, 153 / 255.0f, 255 / 255.0f);
        static readonly Vector8 BallColor = new Vector8(0.8f, 1f, 0.5f);

        public Camera1 cam = Camera1.Create(new Vector1(4, 9, 4), new Vector1(0,0,0));
        static Vector8 sunpos = new Vector8(-10, 10, -10);
        Vector8 sundir = sunpos.Normalize();
        Vector8 ambient = new Vector8(0.1f, 0.1f, 0.1f);

        Plane8 p = new Plane8(new Vector8(0, 0, 0), new Vector8(0, 1, 0));
        Sphere8 s = new Sphere8(new Vector8(0, 1, 0), 1);

        Vector8 CastRay(Ray8 ray)
        {
            f8 dsphere = s.Intersect(ray, out var sphere_normal);
            Vector8 pointsphere = ray.Walk(dsphere);

            f8 dplane = p.Intersect(ray, out var pointplane, out var plane_normal);

            Vector256<int> hitx = Avx2.ConvertToVector256Int32(Avx2.Floor(pointplane.X.E));
            Vector256<int> hitz = Avx2.ConvertToVector256Int32(Avx2.Floor(pointplane.Z.E));
            
            //This line does ((int)x + (int)y) % 2 == 0 ? 0 : 1 for 8 elements
            f8 hitsum = Avx2.ConvertToVector256Single(Avx2.And(Avx2.Add(hitx, hitz), OneInt));

            f8 sunmult = (Vector8.Dot(sundir, ray.Direction) + 1f) / 2;

            Vector8 RGB = Vector8.Lerp(ambient, SkyColor, sunmult);

            f8 shadow = f8.One;
            shadow = (dplane < Hit.NoneCmp).Select(Vector8.Dot((sunpos - pointplane).Normalize(), plane_normal), shadow);
            shadow = (dsphere < Hit.NoneCmp).Select(Vector8.Dot((sunpos - pointsphere).Normalize(), sphere_normal), shadow);


            RGB = (dplane < Hit.NoneCmp).Select(hitsum, RGB);
            RGB = (dsphere < Hit.NoneCmp).Select(BallColor, RGB);

            return Vector8.Lerp(RGB, RGB * ambient, 1 - shadow);
        }


        static readonly Vector1 ZeroTo3 = new Vector1(0, 1, 2, 3);

        public unsafe void Draw(FrameBuffer buffer)
        {
            float aspect = (float)buffer.Height / buffer.Width;
            Vector1 invwidth = new Vector1(1.0f / buffer.Width);

            Vector8 origin = new Vector8(cam.Pos.X, cam.Pos.Y, cam.Pos.Z);
            

            for (int y = 0; y < buffer.Height; y++)
            {
                byte* ptr = ((byte*)buffer.Pointer) + y * buffer.Width * 4;

                float ynorm = ((float)y / -buffer.Height + 0.5f) * 2 * aspect;
                for (int x = -buffer.Width / 2; x < buffer.Width / 2; x+=4, ptr += 16)
                {
                    Vector1 xnorm = ((new Vector1(x) + ZeroTo3) * invwidth) * new Vector1(2);

                    Vector1 x0 = new Vector1(xnorm[0], ynorm, 0);
                    Vector1 x1 = new Vector1(xnorm[1], ynorm, 0);
                    Vector1 x2 = new Vector1(xnorm[2], ynorm, 0);
                    Vector1 x3 = new Vector1(xnorm[3], ynorm, 0);

                    Vector1 directions = cam.Forward + (xnorm * cam.Right) + (ynorm * cam.Up);

                    Ray8 r = new Ray8(origin, new Vector8(directions.X, directions.Y, directions.Z));
                    Vector8 color = CastRay(r);

                    x0 = directions;
                    x1 = directions;
                    x2 = directions;
                    x3 = directions;

                    color.ExportRGBA((float*)ptr);

                    //Vector1.ExportBytes(x0, x1, x2, x3, ptr);
                }
            }
            Avx2.StoreFence();
        }

        /*
        public unsafe void Draw(FrameBuffer8 buffer)
        {
            float aspect = (float)buffer.Height / buffer.Width;

            //Parallel.For(0, buffer.Height, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, (y) =>
            for(int y = 0; y < buffer.Height; y++)
            {
                f8 ynorm = new f8(((float)y / -buffer.Height + 0.5f) * 2 * aspect);
                Vector8 YUp = Vector8.MultiplyAdd(ynorm, cam.Up, cam.Forward);

                float* ptr = ((float*)buffer.Pointer) + y * buffer.Width * 4;

                for (int i = 0; i < Xs.Length; i++, ptr += Vector8.RGBACount)
                {
                    f8 xnorm = Xs[i];
                    //Vector8 directions = cam.Forward + (xnorm * cam.Right) + (ynorm * cam.Up);
                    //directions = directions.Normalize();
                    Vector8 directions = Vector8.MultiplyAdd(xnorm, cam.Right, YUp).Normalize();

                    Ray8 ray = new Ray8(cam.Pos, directions);
                    Vector8 colors = CastRay(ray);

                    colors.ExportRGBA(ptr);
                }
            };

            Avx2.StoreFence();
        }
        */
        
    }
}
