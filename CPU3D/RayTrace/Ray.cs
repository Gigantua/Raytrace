using CPU3D.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using v3 = System.Runtime.Intrinsics.Vector128<float>;
using vmath = System.Runtime.Intrinsics.X86.Avx2;

namespace CPU3D.RayTrace
{
    public readonly struct Ray
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;
        public readonly Vector3 InverseDirection;

        public readonly float RefractiveIndex;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Walk(float Distance)
        {
            float x = Origin.X + Direction.X * Distance;
            float y = Origin.Y + Direction.Y * Distance;
            float z = Origin.Z + Direction.Z * Distance;
            
            return new Vector3(x, y, z);
        }

        public Ray ChangeRefraction(float NewIndex)
        {
            return new Ray(Origin, Direction, NewIndex);
        }

        public Ray(Vector3 Origin, Vector3 Direction, float RefractiveIndex)
        {
            this.Origin = Origin;
            
            this.Direction = Vector3.Normalize(Direction);
            this.InverseDirection = new Vector3(1 / this.Direction.X, 1 / this.Direction.Y, 1 / this.Direction.Z);
            this.RefractiveIndex = RefractiveIndex;
        }

        public Ray(Vector3 Origin, Vector3 Direction, float Offset, float RefractiveIndex)
        {
            this.Direction = Vector3.Normalize(Direction);
            this.Origin = Origin + this.Direction * Offset;
            this.InverseDirection = new Vector3(1 / this.Direction.X, 1 / this.Direction.Y, 1 / this.Direction.Z);
            this.RefractiveIndex = RefractiveIndex;
        }

        public Ray(Vector3 Origin, Vector3 Direction)
        {
            this.Origin = Origin;
            this.Direction = Vector3.Normalize(Direction);
            this.InverseDirection = new Vector3(1 / this.Direction.X, 1 / this.Direction.Y, 1 / this.Direction.Z);
            this.RefractiveIndex = 1;
        }

        Ray(Ray source, float offset)
        {
            this.Origin = source.Origin + source.Direction * offset;
            this.Direction = source.Direction;
            this.InverseDirection = source.InverseDirection;
            this.RefractiveIndex = source.RefractiveIndex;
        }

        public Ray Offset(float distance)
        {
            return new Ray(this, distance); //Special faster constructor
        }


        static readonly Vector2 NotFive = new Vector2(0.5f, 0.5f);
        public static Ray CastScreen(Vector2 ScreenXY, Vector2 ScreenWidthHeight, Camera lens)
        {
            float ratio = ScreenWidthHeight.X / ScreenWidthHeight.Y;
            //-1...1
            // > 100° fov
            /*
            float dX = 2 * ScreenXY.X * ratio / ScreenWidthHeight.X - ratio;
            float dY = 1 - 2 * ScreenXY.Y / ScreenWidthHeight.Y;

            Matrix4x4 rotateX = Matrix4x4.CreateFromAxisAngle(lens.Up, dX * lens.FOV / 2);
            Matrix4x4 rotateY = Matrix4x4.CreateFromAxisAngle(-lens.Right, dY * lens.FOV / 2);

            Vector3 finaldir = Vector3.Transform(lens.Direction, rotateY * rotateX);

            return new Ray(lens.Position, finaldir);
            */
            Vector2 UV = (ScreenXY / ScreenWidthHeight - NotFive) * lens.HalfTanFOV2;
            return CastUV(ratio, UV, lens.Position, lens.Direction, lens.Right, lens.Up);
        }

        /// <summary>
        /// Cast with normalized screen coordinate -1...1, or less depeding on FOV
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ray CastUV(float ratio, Vector2 ScreenUV, Vector3 pos, Vector3 dir, Vector3 right, Vector3 up)
        {
            Vector3 image_point = ScreenUV.X * right * ratio - ScreenUV.Y * up + dir;
            return new Ray(pos, image_point);
        }



    }
}
