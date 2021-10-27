using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CPU3D.RayTrace
{
    public class Matrix3x3
    {
        public Vector3 Row0;
        public Vector3 Row1;
        public Vector3 Row2;

        public static Matrix3x3 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            angle = -angle;
            float x = axis.X, y = axis.Y, z = axis.Z;
            float sa = (float)Math.Sin(angle), ca = (float)Math.Cos(angle);
            float xx = x * x, yy = y * y, zz = z * z;
            float xy = x * y, xz = x * z, yz = y * z;

            Matrix3x3 result = new Matrix3x3();

            var M11 = xx + ca * (1 - xx);
            var M12 = xy - ca * xy + sa * z;
            var M13 = xz - ca * xz - sa * y;
            var M21 = xy - ca * xy - sa * z;
            var M22 = yy + ca * (1 - yy);
            var M23 = yz - ca * yz + sa * x;
            var M31 = xz - ca * xz + sa * y;
            var M32 = yz - ca * yz - sa * x;
            var M33 = zz + ca * (1 - zz);

            result.Row0 = new Vector3(M11, M12, M13);
            result.Row1 = new Vector3(M21, M22, M23);
            result.Row2 = new Vector3(M31, M32, M33);
            return result;
        }

        public static Vector3 Transform(Vector3 a, Matrix3x3 b)
        {
            return new Vector3
                (
                    Vector3.Dot(a, b.Row0),
                    Vector3.Dot(a, b.Row1),
                    Vector3.Dot(a, b.Row2)
                );
        }
    }

    public readonly struct Vector2
    {
        public float X { get; }
        public float Y { get; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }

        public Vector2(float x)
        {
            X = x;
            Y = x;
        }

        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 UnitX => new Vector2(1, 0);
        public static Vector2 UnitY => new Vector2(0, 1);
        public static Vector2 One => new Vector2(1, 1);

        public static Vector2 operator *(float a, Vector2 b)
        {
            return new Vector2(a * b.X, a * b.Y);
        }
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X / b.X, a.Y / b.Y);
        }

        public float LengthSquared() => X * X + Y * Y;

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }
        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.X, -a.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vector3
    {
        public readonly float X, Y, Z;
        public Vector3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public float LengthSquared() => X * X + Y * Y + Z * Z;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public float InverseLength() => FMath.RecSqrt(LengthSquared());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public float Length() => FMath.Sqrt(LengthSquared());

        public static Vector3 Normalize(Vector3 a)
        {
            float inv = a.InverseLength();
            return new Vector3(a.X * inv, a.Y * inv, a.Z * inv);
        }

        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 UnitX = new Vector3(1, 0, 0);
        public static readonly Vector3 UnitY = new Vector3(0, 1, 0);
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
        public static readonly Vector3 One = new Vector3(1, 1, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Max()
        {
            if (X > Y)
            {
                if (X > Z) return X;
                else return Z;
            }
            else if (Y > Z) return Y;
            else return Z;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Min()
        {
            if (X < Y)
            {
                if (X < Z) return X;
                else return Z;
            }
            else if (Y < Z) return Y;
            else return Z;
        }

        public static Vector3 Lerp(Vector3 A, Vector3 B, float amount)
        {
            return A + (B - A) * amount;
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }
        public static Vector3 operator *(float b, Vector3 a)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }


        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            float x = v1.Y * v2.Z - v2.Y * v1.Z;
            float y = v2.X * v1.Z - v1.X * v2.Z;
            float z = v1.X * v2.Y - v2.X * v1.Y;
            return new Vector3(x, y, z);
        }

        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            return (b - a).Length();
        }

        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            return (b - a).LengthSquared();
        }
    }

    public static class FMath
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct FInt
        {
            [FieldOffset(0)] public float x;
            [FieldOffset(0)] public int i;
        }

        public static float Sqrt(float x)
        {
            return Sqrt1000(x);
        }

        public static float Sqrt1000(float x)
        {
            FInt u;
            u.i = 0;
            u.x = x;
            u.i = (1 << 29) + (u.i >> 1) - (1 << 22);
            u.x = u.x + x / u.x;
            u.x = 0.25f * u.x + x / u.x;
            return u.x;
        }

        public static float Sqrt999(float x)
        {
            FInt u;
            u.i = 0;
            u.x = x;
            u.i = (1 << 29) + (u.i >> 1) - (1 << 22);
            return u.x;
        }


        public static unsafe float RecSqrt(float number)
        {
            float x2 = number * 0.5F;
            float y = number;
            long i = *(long*)&y;
            i = 0x5f3759df - (i >> 1);
            y = *(float*)&i;
            y = y * (1.5f - (x2 * y * y));
            y = y * (1.5f - (x2 * y * y));
            y = y * (1.5f - (x2 * y * y)); //Need this for dist > 100 else rays end up inside object on walk
            return y;
        }

    }
}
