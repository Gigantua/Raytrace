using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace CPU3D.Draw
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color8 : IColor<byte, int>, IEquatable<Color8>
    {
        [FieldOffset(0)]
        byte _R;
        [FieldOffset(1)]
        byte _G;
        [FieldOffset(2)]
        byte _B;
        [FieldOffset(3)]
        byte _A;

        [FieldOffset(0)]
        int _RGBA;

        public byte R => _R;
        public byte G => _G;
        public byte B => _B;
        public byte A => _A;
        int IColor<byte, int>.RGB => _RGBA;

        public double Luma()
        {
            return Math.Sqrt(0.299 * R / 255 + 0.587 * G / 255 + 0.114 * B / 255);
        }

        public static implicit operator int(Color8 d)
        {
            return d._RGBA;
        }

        public static implicit operator Color8(int d)
        {
            return new Color8() { _RGBA = d };
        }

        public static implicit operator Color8(ValueTuple<byte, byte, byte, byte> rgba)
        {
            var (x, y, z, w) = rgba;
            return new Color8()
            {
                _R = rgba.Item1,
                _G = rgba.Item2,
                _B = rgba.Item3,
                _A = rgba.Item4,
            };
        }

        public static implicit operator Color8(ValueTuple<byte, byte, byte> rgba)
        {
            var (x, y, z) = rgba;
            return (rgba.Item1, rgba.Item2, rgba.Item3, byte.MaxValue);
        }


        public static Color8 operator *(double r, Color8 c)
        {
            return new Color8()
            {
                _R = (byte)(c.R * r),
                _G = (byte)(c.G * r),
                _B = (byte)(c.B * r),
                _A = (byte)(c.A)
            };
        }
        public static Color8 operator *(Color8 c, double r)
        {
            return new Color8()
            {
                _R = (byte)(c.R * r),
                _G = (byte)(c.G * r),
                _B = (byte)(c.B * r),
                _A = (byte)(c.A)
            };
        }

        public static Color8 Transparent => (0, 0, 0, 0);
        public static Color8 Black => (0,0,0,255);
        public static Color8 Red => (255, 0, 0, 255);
        public static Color8 Green => (0, 255, 0, 255);
        public static Color8 Blue => (0, 0, 255, 255);
        public static Color8 White => (255, 255, 255, 255);
        public static Color8 Yellow => (255, 255, 0, 255);


        public static Color8 Average(params Color8[] colors)
        {
            if (colors == null || colors.Length == 0) return Color8.Black;

            int r = 0, g = 0, b = 0, a = 0;
            for(int i=0;i<colors.Length;i++)
            {
                r += colors[i].R;
                g += colors[i].G;
                b += colors[i].B;
                a += colors[i].A;
            }
            r /= colors.Length;
            g /= colors.Length;
            b /= colors.Length;
            a /= colors.Length;

            return ((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public int ToInt()
        {
            return _RGBA;
        }

        public static bool operator ==(Color8 a, Color8 b)
        {
            return a._RGBA == b._RGBA;
        }

        public static bool operator !=(Color8 a, Color8 b)
        {
            return a._RGBA != b._RGBA;
        }

        public override bool Equals(object obj)
        {
            if (obj is Color8 c)
            {
                return c == this;
            }
            else return base.Equals(obj);
        }

        public bool Equals(Color8 other)
        {
            return _R == other._R &&
                   _G == other._G &&
                   _B == other._B;
        }

        public override int GetHashCode()
        {
            var hashCode = 2081209147;
            hashCode = hashCode * -1521134295 + _R.GetHashCode();
            hashCode = hashCode * -1521134295 + _G.GetHashCode();
            hashCode = hashCode * -1521134295 + _B.GetHashCode();
            return hashCode;
        }
    }
}
