using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using fmath = System.Runtime.Intrinsics.X86.Fma;
using vec4 = System.Runtime.Intrinsics.Vector128<float>;

namespace RayForm
{
    public readonly struct Vector1
    {
        static readonly Vector128<float> ExpC = Vector128.Create(255.0f);

        public readonly vec4 E;

        public override string ToString()
        {
            return $"{this[0].ToString("0.00")}|{this[1].ToString("0.00")}|{this[2].ToString("0.00")}|{this[3].ToString("0.00")}";
        }

        public Vector1(float x, float y, float z, float w)
        {
            E = Vector128.Create(x, y, z, w);
        }
        public Vector1(float x, float y, float z)
        {
            E = Vector128.Create(x, y, z, 0);
        }
        public Vector1(float x)
        {
            E = Vector128.Create(x);
        }
        public Vector1(vec4 Elements)
        {
            E = Elements;
        }

        public float this[int index]
        {
            get => E.GetElement(index);
        }

        public static implicit operator Vector1(vec4 Elements) => new Vector1(Elements);

        public static Vector1 MultiplyAdd(Vector1 a, Vector1 b, Vector1 add)
        {
            return fmath.MultiplyAdd(a.E, b.E, add.E);
        }

        public static Vector1 operator+(Vector1 a, Vector1 b)
        {
            return fmath.Add(a.E, b.E);
        }
        public static Vector1 operator -(Vector1 a, Vector1 b)
        {
            return fmath.Subtract(a.E, b.E);
        }

        public static float Dot(Vector1 a, Vector1 b)
        {
            return fmath.DotProduct(a.E, b.E, 0xFF).GetElement(0);
        }

        public static Vector1 operator *(Vector1 a, Vector1 b)
        {
            return fmath.Multiply(a.E, b.E);
        }
        public static Vector1 operator *(Vector1 a, float b)
        {
            return fmath.Multiply(a.E, Vector128.Create(b));
        }
        public static Vector1 operator *(float a, Vector1 b)
        {
            return fmath.Multiply(Vector128.Create(a), b.E);
        }

        public static Vector1 Cross(Vector1 a, Vector1 b)
        {
            vec4 a_yzx = fmath.Shuffle(a.E, a.E, 0b11001001);
            vec4 b_yzx = fmath.Shuffle(b.E, b.E, 0b11001001);
            vec4 c = fmath.Subtract(fmath.Multiply(a.E, b_yzx), fmath.Multiply(a_yzx, b.E));
            return fmath.Shuffle(c, c, 0b11001001); //_MM_SHUFFLE(3, 0, 2, 1);
        }

        public float Length => (float)Math.Sqrt(fmath.DotProduct(E, E, 0xFF).GetElement(0));
        public float LengthSquared => fmath.DotProduct(E, E, 0xFF).GetElement(0);
        public float InvLength => 1.0f / Length;

        public float X => E.GetElement(0);
        public float Y => E.GetElement(1);
        public float Z => E.GetElement(2);


        public Vector1 Normalize()
        {
            return this * InvLength;
        }

        public unsafe void ExportFloats(float* ptr)
        {
            Avx2.StoreAlignedNonTemporal(ptr, E);
        }

        public static unsafe void ExportBytes(Vector1 a, Vector1 b, Vector1 c, Vector1 d, byte* ptr)
        {
            Vector128<int> A = Avx2.ConvertToVector128Int32(Avx2.Multiply(a.E, ExpC));
            Vector128<int> B = Avx2.ConvertToVector128Int32(Avx2.Multiply(b.E, ExpC));
            Vector128<int> C = Avx2.ConvertToVector128Int32(Avx2.Multiply(c.E, ExpC));
            Vector128<int> D = Avx2.ConvertToVector128Int32(Avx2.Multiply(d.E, ExpC));
            var p0 = Avx2.PackSignedSaturate(A, B);
            var p1 = Avx2.PackSignedSaturate(C, D);
            Vector128<byte> elements = Avx.PackUnsignedSaturate(p0, p1);
            Avx.StoreAlignedNonTemporal(ptr, elements);
        }
    }
}
