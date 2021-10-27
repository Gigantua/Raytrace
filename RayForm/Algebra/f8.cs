using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace RayForm
{
    public readonly struct fbool
    {
        public readonly Vector256<float> M;
        public fbool(Vector256<float> mask) => M = mask;

        public static fbool operator &(fbool a, fbool b) => new fbool(Avx2.And(a.M, b.M));
        public static fbool operator |(fbool a, fbool b) => new fbool(Avx2.Or(a.M, b.M));
        public static fbool operator !(fbool a) => new fbool(Avx2.Xor(a.M, Vector256.Create(-1).AsSingle()));

        public f8 Select(f8 a, f8 b)
        {
            return Avx2.BlendVariable(b.E, a.E, M);
        }

        public Vector8 Select(f8 a, Vector8 b)
        {
            if (AllFalse) return b;
            f8 x = Avx2.BlendVariable(b.X.E, a.E, M);
            f8 y = Avx2.BlendVariable(b.Y.E, a.E, M);
            f8 z = Avx2.BlendVariable(b.Z.E, a.E, M);
            return new Vector8(x, y, z);
        }
        public Vector8 Select(Vector8 a, Vector8 b)
        {
            if (AllFalse) return b;
            f8 x = Avx2.BlendVariable(b.X.E, a.X.E, M);
            f8 y = Avx2.BlendVariable(b.Y.E, a.Y.E, M);
            f8 z = Avx2.BlendVariable(b.Z.E, a.Z.E, M);
            return new Vector8(x, y, z);
        }

        public unsafe void Unpack(Span<int> selector)
        {
            fixed (int* ptr = selector)
            {
                Avx2.Store(ptr, M.AsInt32());
            }
        }

        public static fbool True = new fbool(Vector256.Create(-1).AsSingle());
        public static fbool False = new fbool(Vector256.Create(0).AsSingle());

        public bool this[int index] => M.GetElement(index) != 0;

        public bool AllFalse => Avx2.TestZ(M, M);
        public bool AllTrue
        {
            get
            {
                var mask = Avx2.CompareEqual(M.AsByte(), Vector256.Create((byte)0xFF));
                return Avx2.TestZ(mask, mask);
            }
        }
    }

    public readonly struct f8
    {
        public readonly Vector256<float> E;

        public static readonly f8 Zero = new f8(0);
        public static readonly f8 One = new f8(1);
        public static readonly f8 ZeroTo7 = new f8(0, 1, 2, 3, 4, 5, 6, 7);

        public override string ToString() => E.ToString();
        public f8(Vector256<float> Elements) => E = Elements;
        public f8(float AllElements) => E = Vector256.Create(AllElements);
        public unsafe f8(float* AllElements) => E = Avx2.LoadVector256(AllElements);
        public f8(float a, float b, float c, float d, float e, float f, float g, float h) => E = Vector256.Create(a, b, c, d, e, f, g, h);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator f8(float Element) => new f8(Element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator f8(Vector256<float> Elements) => new f8(Elements);

        public static f8 Create(float Values)
        {
            return new f8(Vector256.Create(Values));
        }

        //latency, throughput
        public static f8 Sqrt(f8 a) => Avx2.Sqrt(a.E); //12, 6
        public static f8 RecSqrt(f8 a) => Avx2.ReciprocalSqrt(a.E); //4, 1
        public static f8 FastSqrt(f8 a) => Avx2.Reciprocal(Avx2.ReciprocalSqrt(a.E)); //8,2

        public static unsafe f8 Load(float* Source) => new f8(Source);

        public float GetElement(int index) => E.GetElement(index);

        public unsafe void Store(float* Target) => Avx2.Store(Target, E);

        public static unsafe f8 LoadAligned32(float* Source) => new f8(Avx2.LoadAlignedVector256(Source));
        public unsafe void StoreAligned32(float* Target) => Avx2.StoreAlignedNonTemporal(Target, E);

        static fbool Compare(f8 a, f8 b, FloatComparisonMode mode)
        {
            return new fbool(Avx2.Compare(a.E, b.E, mode));
        }

        public static fbool operator ==(f8 a, f8 b) => Compare(a.E, b.E, FloatComparisonMode.OrderedEqualNonSignaling);
        public static fbool operator !=(f8 a, f8 b) => Compare(a.E, b.E, FloatComparisonMode.OrderedNotEqualNonSignaling);
        public static fbool operator >(f8 a, f8 b) => Compare(a.E, b.E, FloatComparisonMode.OrderedGreaterThanNonSignaling);
        public static fbool operator <(f8 a, f8 b) => Compare(a.E, b.E, FloatComparisonMode.OrderedLessThanNonSignaling);
        public static fbool operator <=(f8 a, f8 b) => Compare(a.E, b.E, FloatComparisonMode.OrderedLessThanOrEqualNonSignaling);
        public static fbool operator >=(f8 a, f8 b) => Compare(a.E, b.E, FloatComparisonMode.OrderedGreaterThanOrEqualNonSignaling);

        static readonly Vector256<int> SignMask = Vector256.Create(-1);
        public static f8 Abs(f8 a) => Avx2.Or(SignMask, a.E.AsInt32()).AsSingle();


        public static f8 operator -(f8 a) => Avx2.Subtract(Zero.E, a.E);
        public static f8 operator -(f8 a, f8 b) => Avx2.Subtract(a.E, b.E);
        public static f8 operator +(f8 a, f8 b) => Avx2.Add(a.E, b.E);
        public static f8 operator *(f8 a, f8 b) => Avx2.Multiply(a.E, b.E);
        public static f8 operator /(f8 a, f8 b) => Avx2.Divide(a.E, b.E);
        public static f8 MultiplyAdd(f8 a, f8 b, f8 add) => new f8(Fma.MultiplyAdd(a.E, b.E, add.E));

        public static f8 Min(f8 a, f8 b) => Avx2.Min(a.E, b.E);
        public static f8 Max(f8 a, f8 b) => Avx2.Max(a.E, b.E);
    }
}
