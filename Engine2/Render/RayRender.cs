using Engine2.Primitives;
using Engine2.Raytrace;
using SharpDX;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Render
{
    [Flags]
    enum CastResult
    {
        Surface = 0,
        Infinity = 1,
        Object = 2,
        Light = 4,
        Recursion = 8
    }

    class RayResult
    {
        public CastResult type;
        public Vector3 HitPoint;
        public Triangle HitVertice;
        public Plane HitSurface;
        public float Brightness;

        public RayResult(CastResult result)
        {
            this.type = result;
        }

        public static RayResult Recursion => new RayResult(CastResult.Recursion);
        public static RayResult Infinity => new RayResult(CastResult.Infinity);

        public static RayResult FromLight(Vector3 Ligthposition,Vector3 hitlocation)
        {
            return new RayResult(CastResult.Light) { HitPoint = hitlocation };
        }

        public static RayResult FromSurface(Plane surface,Vector3 hitlocation, float LightAmount)
        {
            return new RayResult(CastResult.Surface) { HitSurface = surface, HitPoint = hitlocation, Brightness = LightAmount };
        }
        public static RayResult FromObject(Triangle face, Vector3 hitlocation, float LightAmount)
        {
            return new RayResult(CastResult.Object) { HitVertice = face, HitPoint = hitlocation, Brightness = LightAmount };
        }
    }

    class RayRender
    {
        Bitmap_float Target;
        GameObject[] Objects;
        int Width;
        int Height;
        ZBuffer ZBuf;
        Matrix Projectionview;
        Camera cam;
        GameEngine Engine;

        Vector3 lightpos;
        BoundingSphere lightsphere;

        Plane floor = new Plane(new Vector3(0, -1, 0), new Vector3(0, 1, 0));
        Matrix3x3 floorreflect;

        float FloorReflect;
        float ObjReflect;

        public RayRender(Bitmap_float Target, ZBuffer zbuf, Camera cam, GameObject[] Objects, GameEngine Engine)
        {
            this.Width = Target.Width;
            this.Height = Target.Height;
            this.ZBuf = zbuf;
            this.Target = Target;
            this.Objects = Objects;
            this.cam = cam;
            this.Engine = Engine;

            floorreflect = floor.Reflection3x3();
            Sky.A = 0.5f;
        }

        public void Draw()
        {
            FloorReflect = (float)Properties.Settings.Default.Floor_Reflectiveness / 100.0f;
            ObjReflect = (float)Properties.Settings.Default.Object_Reflectiveness / 100.0f;

            lightpos = Engine.LightPos;
            lightsphere = new BoundingSphere(lightpos, Engine.LightDiameter);
            float r = Engine.LightDiameter;

            var ViewMatrix = Matrix.LookAtLH(cam.Position, cam.Target, Vector3.UnitY);
            var ProjectMatrix = Matrix.PerspectiveFovLH(1f, (float)Width / Height, 0.01f, 1.0f);
            Projectionview = ViewMatrix * ProjectMatrix; //Is a matrix for unmovable objects

            for (int i = 0; i < Objects.Length; i++)
            {
                Draw(Objects[i]);
            }
        }

        public void Draw(GameObject obj)
        {
            var worldviewprojection = obj.TransformMatrix * Projectionview;

            Color4[] backbuf = Target.Pixel;
            var viewport = new Viewport(-Width / 2, -Height / 2, 2 * Width, 2 * Height, 0.01f, 1f);
            var faces = obj.Faces;

            ConcurrentBag<(int x, int y)> Interpolated = new ConcurrentBag<(int x, int y)>();
            Parallel.For(0, Height, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount + 2 }, (y) =>
            {
                int width = this.Width;
                for (int x = 0; x < width; x++)
                {
                    Color4? result = CastRay(obj, worldviewprojection, Ray.GetPickRay(x, y, viewport, worldviewprojection), 0);
                    if (result == null) Interpolated.Add((x, y));
                    else backbuf[y * width + x] = Color4.Clamp(result.Value);
                }
            });
            
            foreach (var (x,y) in Interpolated)
            {
                backbuf[y * Width + x] = Target.AverageAround(x, y);
            }
        }

        public float CosineBetween(Vector3 a, Vector3 b)
        {
            return Vector3.Dot(a, b) / (a.Length() * b.Length());
        }
        public float SineBetween(Vector3 a, Vector3 b)
        {
            return Vector3.Cross(a, b).Length() / (a.Length() * b.Length());
        }

        public float CCosineBetween(Vector3 a, Vector3 b)
        {
            return Math.Max(0, CosineBetween(a, b)); //triangles looking away are dark
        }

        static Color4 Sky = Colorconvert.FromColor(System.Drawing.Color.SkyBlue);
        static Color4 LightColor = Colorconvert.FromColor(System.Drawing.Color.LightYellow);

        Color4 GetDirectIllumination(Vector3 LightPos,Vector3 HitPos)
        {
            Ray r = new Ray(HitPos, Vector3.Normalize(LightPos - HitPos));

            return Color4.FromElements(0, 0, 0, 0);
        }

        Color4 FloorColor(Vector3 floorpoint)
        {
            float a = floorpoint.X % 1f;
            float b = floorpoint.Z % 1f;

            if (a > 0) a -= 0.5f;
            else a += 0.5f;

            if (b > 0) b -= 0.5f;
            else b += 0.5f;

            if (Math.Sign(a) == Math.Sign(b))
            {
                return Color4.FromElements(0, 0, 0, 1f);
            }
            else
            {
                return Color4.FromElements(1, 1, 1, 1f);
            }
        }

        float ObjectLuminance(Vector3 position,Triangle intersection, float LightAmount)
        {
            if (LightAmount == 0) return 0;

            float rA = Vector3.DistanceSquared(lightpos, intersection.TA.WorldCoords) / 150;
            float rB = Vector3.DistanceSquared(lightpos, intersection.TB.WorldCoords) / 150;
            float rC = Vector3.DistanceSquared(lightpos, intersection.TC.WorldCoords) / 150;

            float lumaA = (CCosineBetween(intersection.TA.Normal, lightpos - intersection.TA.WorldCoords)) / rA; //lightdir or lightpos - coord for pointlight
            float lumaB = (CCosineBetween(intersection.TB.Normal, lightpos - intersection.TB.WorldCoords)) / rB;
            float lumaC = (CCosineBetween(intersection.TC.Normal, lightpos - intersection.TC.WorldCoords)) / rC;
            Vector3 lumi = new Vector3(lumaA, lumaB, lumaC);

            Vector3 UVW = intersection.Barycentric(position);

            float radiance = UVW[0] * lumaA + UVW[1] * lumaB + UVW[2] * lumaC;
            return radiance * LightAmount;
        }

        float SurfaceLuminance(Vector3 position, float LightAmount)
        {
            if (LightAmount == 0) return 0;

            float luma = CCosineBetween(Vector3.UnitY, Vector3.Normalize(lightpos - position));
            float d2 = Vector3.DistanceSquared(position, lightpos) / 150;

            return luma / d2 * LightAmount;
        }

        Ray BounceSurface(Plane p,Vector3 HitPos,Ray r)
        {
            double dX = Math.Sin(HitPos.X);
            double dZ = Math.Cos(HitPos.Z);

            Vector3 wave = new Vector3((float)(dX), 0, (float)(dZ));
            float Timefact = (float)(Math.Cos(Engine.TotalTime*5)) * 0.1f; //0
            Matrix3x3 rot = Matrix3x3.RotationAxis(wave, Timefact);

            Vector3 newdir = Vector3.Normalize(Vector3.Transform(r.Direction, p.Reflection3x3()));
            newdir = Vector3.Transform(newdir, rot);
            return new Ray(HitPos + newdir * 0.01f, newdir);
        }

        Color4 SkyBoxLookup(int side,float X, float Y)
        {
            if (side==1 || side == 4)
            {
                return Engine.SkyBox_Sides[side].GetPixel(1-X, 1 - Y);
            }
            if (side == 2)
            {
                return Engine.SkyBox_Sides[side].GetPixel(1-Y,X);
            }
            if (side == 3)
            {
                return Engine.SkyBox_Sides[side].GetPixel(1-Y, 1-X);
            }
            return Engine.SkyBox_Sides[side].GetPixel(X, 1-Y);
        }


        Color4 SkyBox(Ray r)
        {
            Vector3 dr = new Vector3(10000, 10000, 10000);
            Ray infinityray = new Ray(r.Direction * 20000, -r.Direction);
            Vector3 min = Vector3.Add(-dr, Vector3.Zero);
            Vector3 max = Vector3.Add(dr, Vector3.Zero);
            BoundingBox SkyBox = new BoundingBox(min,max);

            Vector3 intersectpos;
            Collision.RayIntersectsBox(ref infinityray, ref SkyBox, out intersectpos);

            float epsilon = 0.5f;
            float dX = max.X - min.X;
            float dY = max.Y - min.Y;
            float dZ = max.Z - min.Z;

            if (Math.Abs(intersectpos.X - min.X) < epsilon) //left
            {
                return SkyBoxLookup(0, (intersectpos.Z - min.Z) / dZ, (intersectpos.Y - min.Y) / dY);
            }
            else if (Math.Abs(intersectpos.X - max.X) < epsilon) //right
            {
                return SkyBoxLookup(1, (intersectpos.Z - min.Z) / dZ, (intersectpos.Y - min.Y) / dY);
            }
            else if (Math.Abs(intersectpos.Y - min.Y) < epsilon) //bottom
            {
                return SkyBoxLookup(2, (intersectpos.X - min.X)/dX, 1-(intersectpos.Z - min.Z) / dZ);
            }
            else if (Math.Abs(intersectpos.Y - max.Y) < epsilon) //top
            {
                return SkyBoxLookup(3, (intersectpos.X - min.X) / dX, 1-(intersectpos.Z - min.Z) / dZ);
            }
            else if (Math.Abs(intersectpos.Z - min.Z) < epsilon) //back
            {
                return SkyBoxLookup(4, (intersectpos.X - min.X) / dX, (intersectpos.Y - min.Y) / dY);
            }
            else if (Math.Abs(intersectpos.Z - max.Z) < epsilon) //front
            {
                return SkyBoxLookup(5, (intersectpos.X - min.X) / dX, (intersectpos.Y - min.Y) / dY);
            }
            return Color4.FromElements(0, 0, 0, 0); //NOT POSSIBLE
        }

        //If we hit object we cast reflection and check direct lighning
        Color4? CastRay(GameObject obj, Matrix objmatrix, Ray r, int recursion)
        {
            RayResult Info = CastSingleRay(obj, objmatrix, r, recursion);
            
            float brightness = 0;
            Color4 Self;
            Color4? Reflection;
            Color4? Refraction;
            Color4 Light = LightColor;

            switch (Info.type)
            {
                case CastResult.Infinity:
                    return SkyBox(r);

                case CastResult.Object:
                    brightness = ObjectLuminance(Info.HitPoint, Info.HitVertice, Info.Brightness);
                    Self = Color4.FromElements(1f, 1f, 1f, 1f);

                    if (ObjReflect == 0) return Self * brightness + Colors.Transparent; //dont cast ray if we dont need color

                    Reflection = CastRay(obj, objmatrix, Info.HitVertice.BounceRay(Info.HitPoint, r), recursion+1);
                    //Refraction = CastRay(obj, objmatrix, Info.HitVertice.RefractRay(Info.HitPoint, r), recursion + 1);

                    return (Self * (1 - ObjReflect) * brightness + Reflection * ObjReflect);

                case CastResult.Surface:
                    brightness = SurfaceLuminance(Info.HitPoint, Info.Brightness);
                    Self = FloorColor(Info.HitPoint);
                    if (FloorReflect == 0) return Self * brightness;

                    Reflection = CastRay(obj, objmatrix, BounceSurface(floor, Info.HitPoint, r), recursion + 1);
                    return (Self * (1 - FloorReflect) * brightness + Reflection * FloorReflect);

                case CastResult.Light:
                    return LightColor;

                default:
                    return null;
            }
        }


        float GetDirectLight(Vector3 Pos, GameObject obj, Matrix objmatrix)
        {
            Vector3 d1 = Vector3.Normalize(lightpos - Pos);

            if (obj.Bounds.IsIntersected(new Ray(Pos +d1*0.01f, d1))) return 0;

            return 1;
        }

        RayResult CastSingleRay(GameObject obj, Matrix objmatrix, Ray r, int recursion)
        {
            if (recursion == 4) return RayResult.Recursion;
            
            Vector3 lightintersection;
            float? LightsDist = null;
            if (Collision.RayIntersectsSphere(ref r, ref lightsphere, out lightintersection))
            {
                LightsDist = Vector3.DistanceSquared(r.Position, lightintersection);
            }

            Vector3 floorpoint;
            float? Floordist = null;
            if (Collision.RayIntersectsPlane(ref r, ref floor, out floorpoint))
            {
                Floordist = Vector3.DistanceSquared(r.Position, floorpoint);
            }
           
            Vector3 objpoint = Vector3.Zero;
            Triangle intersection = null;
            float? ObjDist = null;

            intersection = obj.Bounds.Intersect(r);
            if (intersection != null)
            {
                intersection.Transform(obj.TransformMatrix);
                Collision.RayIntersectsTriangle(ref r, ref intersection.Elements[0], ref intersection.Elements[1], ref intersection.Elements[2], out objpoint);
                ObjDist = Vector3.DistanceSquared(r.Position, objpoint);
            }

            if (Floordist == null && LightsDist == null && ObjDist == null) return RayResult.Infinity; //No Hit

            int smallest;
            if (LightsDist == null && Floordist == null)
                smallest = 3;
            else if (Floordist == null && ObjDist == null)
                smallest = 1;
            else if (LightsDist == null && ObjDist == null)
                smallest = 2;
            else
            {
                if (Floordist==null)
                {
                    smallest = LightsDist < ObjDist ? 2 : 3;
                }
                else if (LightsDist == null)
                {
                    smallest = Floordist < ObjDist ? 1 : 3;
                }
                else
                {
                    smallest = Floordist < LightsDist ? 2 : 1;
                }
            }

            float light;
            switch (smallest)
            {
                case 1:
                    return RayResult.FromLight(lightpos, lightintersection);
                case 2:
                    light = GetDirectLight(floorpoint, obj, objmatrix);
                    return RayResult.FromSurface(floor, floorpoint, light);
                case 3:
                    light = GetDirectLight(objpoint, obj, objmatrix);
                    return RayResult.FromObject(intersection, objpoint, light);
            }


            return RayResult.Infinity;
        }

    }
}
