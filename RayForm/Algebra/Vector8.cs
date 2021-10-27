using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Text;
using System.Runtime.InteropServices;
using f8 = System.Runtime.Intrinsics.Vector256<float>;
using System.Runtime.CompilerServices;

namespace RayForm
{
    public class Vector8
    {
        public readonly f8 X;
        public readonly f8 Y;
        public readonly f8 Z;

        public static readonly Vector8 Zero = new Vector8(0, 0, 0);
        public static readonly Vector8 UnitX = new Vector8(1, 0, 0);
        public static readonly Vector8 UnitY = new Vector8(0, 1, 0);
        public static readonly Vector8 UnitZ = new Vector8(0, 0, 1);
        public static readonly Vector8 One = new Vector8(1, 1, 1);

        public override string ToString()
        {
            return $"{X.E.GetElement(0).ToString("0.00")};{Y.E.GetElement(0).ToString("0.00")};{Z.E.GetElement(0).ToString("0.00")}";
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector8 MultiplyAdd(f8 a, Vector8 b, Vector8 add)
        {
            f8 x = f8.MultiplyAdd(a, b.X, add.X);
            f8 y = f8.MultiplyAdd(a, b.Y, add.Y);
            f8 z = f8.MultiplyAdd(a, b.Z, add.Z);
            return new Vector8(x, y, z);
        }


        public Vector8(f8 X, f8 Y, f8 Z)
        {
            this.X = X; this.Y = Y; this.Z = Z;
        }

        public static Vector8 Lerp(Vector8 a, Vector8 b, f8 factor)
        {
            return a + (b - a) * factor;
        }

        public static Vector8 Cross(Vector8 a, Vector8 b)
        {
            f8 x = a.Y * b.Z - a.Z * b.Y;
            f8 y = a.Z * b.X - a.X * b.Z;
            f8 z = a.X * b.Y - a.Y * b.X;
            return new Vector8(x, y, z);
        }

        public static f8 DistanceSquared(Vector8 a, Vector8 b) => (b - a).LengthSquared;

        public static f8 Dot(Vector8 a, Vector8 b)
        {
            return Fma.MultiplyAdd(a.X.E, b.X.E, Fma.MultiplyAdd(a.Y.E, b.Y.E, Fma.Multiply(a.Z.E, b.Z.E)));
        }

        public static f8 RecDot(Vector8 a, Vector8 b)
        {
            return Fma.Reciprocal(Fma.MultiplyAdd(a.X.E, b.X.E, Fma.MultiplyAdd(a.Y.E, b.Y.E, Fma.Multiply(a.Z.E, b.Z.E))));
        }

        public static f8 Dot(Vector8 a)
        {
            Vector256<float> x = a.X.E;
            Vector256<float> y = a.Y.E;
            Vector256<float> z = a.Z.E;

            return Fma.MultiplyAdd(x, x, Fma.MultiplyAdd(y, y, Fma.Multiply(z, z)));
        }

        public static f8 DotA_AddB(Vector8 a, f8 b)
        {
            Vector256<float> x = a.X.E;
            Vector256<float> y = a.Y.E;
            Vector256<float> z = a.Z.E;

            return Fma.MultiplyAdd(x, x, Fma.MultiplyAdd(y, y, Fma.MultiplyAdd(z, z, b.E)));
        }

        public static f8 DotA_SubB(Vector8 a, f8 b)
        {
            Vector256<float> x = a.X.E;
            Vector256<float> y = a.Y.E;
            Vector256<float> z = a.Z.E;

            return Fma.MultiplyAdd(x, x, Fma.MultiplyAdd(y, y, Fma.MultiplySubtract(z, z, b.E)));
        }

        public Vector8 Normalize()
        {
            f8 rec = RecLenght;
            f8 x = X * rec;
            f8 y = Y * rec;
            f8 z = Z * rec;
            return new Vector8(x, y, z);
        }

        public f8 RecLenght => Avx2.ReciprocalSqrt(LengthSquared.E);
        public f8 Length => Avx2.Sqrt(LengthSquared.E);
        public f8 LengthSquared => X * X + Y * Y + Z * Z;

        public const int RGBACount = 32;

        public static Vector8 operator +(Vector8 a, Vector8 b)
        {
            f8 x = a.X + b.X;
            f8 y = a.Y + b.Y;
            f8 z = a.Z + b.Z;
            return new Vector8(x, y, z);
        }


        public static Vector8 operator -(Vector8 a)
        {
            return new Vector8(-a.X, -a.Y, -a.Z);
        }

        public static Vector8 operator -(Vector8 a, Vector8 b)
        {
            f8 x = a.X - b.X;
            f8 y = a.Y - b.Y;
            f8 z = a.Z - b.Z;
            return new Vector8(x, y, z);
        }

        public static Vector8 operator *(Vector8 a, Vector8 b)
        {
            f8 x = a.X * b.X;
            f8 y = a.Y * b.Y;
            f8 z = a.Z * b.Z;
            return new Vector8(x, y, z);
        }
        public static Vector8 operator *(Vector8 a, float b)
        {
            f8 x = a.X * b;
            f8 y = a.Y * b;
            f8 z = a.Z * b;
            return new Vector8(x, y, z);
        }
        public static Vector8 operator *(float a, Vector8 b)
        {
            f8 x = a * b.X;
            f8 y = a * b.Y;
            f8 z = a * b.Z;
            return new Vector8(x, y, z);
        }

        public static Vector8 operator *(Vector8 a, f8 b)
        {
            f8 x = a.X * b;
            f8 y = a.Y * b;
            f8 z = a.Z * b;
            return new Vector8(x, y, z);
        }

        public static Vector8 operator *(f8 a, Vector8 b)
        {
            f8 x = a * b.X;
            f8 y = a * b.Y;
            f8 z = a * b.Z;
            return new Vector8(x, y, z);
        }


        public static Vector8 operator /(Vector8 a, Vector8 b)
        {
            f8 x = a.X / b.X;
            f8 y = a.Y / b.Y;
            f8 z = a.Z / b.Z;
            return new Vector8(x, y, z);
        }


        unsafe float[] Read(float* E0)
        {
            float[] data = new float[32];
            for (int i = 0; i < 32; i++) data[i] = E0[i];
            return data;
        }

        
        public unsafe void ExportRGBA2(float* E0)
        {
            var xm = Vector256.Create(-1, 0, 0, 0, -1, 0, 0, 0).AsSingle();
            var ym = Vector256.Create(0, -1, 0, 0, 0, -1, 0, 0).AsSingle();
            var zm = Vector256.Create(0, 0, -1, 0, 0, 0, -1, 0).AsSingle();

            var x0 = Avx2.PermuteVar8x32(X.E, Vector256.Create(0, 0, 0, 0, 1, 0, 0, 0)); Avx2.MaskStore(E0 + 0, xm, x0);
            var y0 = Avx2.PermuteVar8x32(Y.E, Vector256.Create(0, 0, 0, 0, 0, 1, 0, 0)); Avx2.MaskStore(E0 + 0, ym, y0);
            var z0 = Avx2.PermuteVar8x32(Z.E, Vector256.Create(0, 0, 0, 0, 0, 0, 1, 0)); Avx2.MaskStore(E0 + 0, zm, z0);

            var x1 = Avx2.PermuteVar8x32(X.E, Vector256.Create(2, 0, 0, 0, 3, 0, 0, 0)); Avx2.MaskStore(E0 + 8, xm, x1);
            var y1 = Avx2.PermuteVar8x32(Y.E, Vector256.Create(0, 2, 0, 0, 0, 3, 0, 0)); Avx2.MaskStore(E0 + 8, ym, y1);
            var z1 = Avx2.PermuteVar8x32(Z.E, Vector256.Create(0, 0, 2, 0, 0, 0, 3, 0)); Avx2.MaskStore(E0 + 8, zm, z1);

            var x2 = Avx2.PermuteVar8x32(X.E, Vector256.Create(4, 0, 0, 0, 5, 0, 0, 0)); Avx2.MaskStore(E0 + 16, xm, x2);
            var y2 = Avx2.PermuteVar8x32(Y.E, Vector256.Create(0, 4, 0, 0, 0, 5, 0, 0)); Avx2.MaskStore(E0 + 16, ym, y2);
            var z2 = Avx2.PermuteVar8x32(Z.E, Vector256.Create(0, 0, 4, 0, 0, 0, 5, 0)); Avx2.MaskStore(E0 + 16, zm, z2);

            var x3 = Avx2.PermuteVar8x32(X.E, Vector256.Create(6, 0, 0, 0, 7, 0, 0, 0)); Avx2.MaskStore(E0 + 24, xm, x3);
            var y3 = Avx2.PermuteVar8x32(Y.E, Vector256.Create(0, 6, 0, 0, 0, 7, 0, 0)); Avx2.MaskStore(E0 + 24, ym, y3);
            var z3 = Avx2.PermuteVar8x32(Z.E, Vector256.Create(0, 0, 6, 0, 0, 0, 7, 0)); Avx2.MaskStore(E0 + 24, zm, z3); 
        }
        

        public unsafe void ExportRGBA(float* E0)
        {
            float* T = stackalloc float[24+8];
            //int offset = (int)(new IntPtr(T).ToInt64() & 31);
            //T = (float*)(((byte*)T) + offset);

            Avx2.Store(T, X.E); Avx2.Store(T+8, Y.E); Avx2.Store(T+16, Z.E);

            E0[0] = T[0]; E0[1] = T[8]; E0[2] = T[16];
            E0[4] = T[1]; E0[5] = T[9]; E0[6] = T[17];
            E0[8] = T[2]; E0[9] = T[10]; E0[10] = T[18];
            E0[12] = T[3]; E0[13] = T[11]; E0[14] = T[19];
            E0[16] = T[4]; E0[17] = T[12]; E0[18] = T[20];
            E0[20] = T[5]; E0[21] = T[13]; E0[22] = T[21];
            E0[24] = T[6]; E0[25] = T[14]; E0[26] = T[22];
            E0[28] = T[7]; E0[29] = T[15]; E0[30] = T[23];
        }
    }
}
