using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    /// <summary>
    /// Subpixels are 10bit 0 to 1023
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct Color10 : IColor<ushort, int>
    {
        [FieldOffset(0)]
        int _RGBA;

        public ushort R => (ushort)((_RGBA >> 0) & 0b1111111111);

        public ushort G => (ushort)((_RGBA >> 10) & 0b1111111111);

        public ushort B => (ushort)((_RGBA >> 20) & 0b1111111111);

        public ushort A => (ushort)((_RGBA >> 30) & 0b0000000011);

        public int RGB => _RGBA;

        public Color8 ToColor8()
        {
            throw new NotImplementedException();
        }

        public int ToInt()
        {
            return _RGBA;
        }

        public static implicit operator int(Color10 d)
        {
            return d._RGBA;
        }

        public static implicit operator Color10(int d)
        {
            return new Color10()
            {
                _RGBA = d
            };
        }

        public static implicit operator Color10(ValueTuple<ushort, ushort, ushort, ushort> rgba)
        {
            var (x, y, z, w) = rgba;
            int rgbaval = 0;
            rgbaval |= (x & 0b1111111111) << 0;
            rgbaval |= (y & 0b1111111111) << 10;
            rgbaval |= (z & 0b1111111111) << 20;
            rgbaval |= (w & 0b11) << 30;

            return new Color10()
            {
                _RGBA = rgbaval
            };
        }

        public static implicit operator Color10(ValueTuple<ushort, ushort, ushort> rgba)
        {
            var (x, y, z) = rgba;
            return (rgba.Item1, rgba.Item2, rgba.Item3, byte.MaxValue);
        }
    }
}
