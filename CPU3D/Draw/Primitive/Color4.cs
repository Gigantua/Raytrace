using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color4 : IColor<byte, short>
    {
        [FieldOffset(0)]
        byte RG;
        [FieldOffset(1)]
        byte BA;
        [FieldOffset(0)]
        short _RGBA;

        public byte R => (byte) (RG & 0x0F);

        public byte G => (byte)((RG & 0xF0) >> 4);

        public byte B => (byte)(BA & 0x0F);

        public byte A => (byte)((BA & 0xF0) >> 4);

        public short RGB => _RGBA;


        public Color8 ToColor8()
        {
            throw new NotImplementedException();
        }

        public int ToInt()
        {
            return (int)_RGBA;
        }

        public static implicit operator int(Color4 d)
        {
            return d._RGBA;
        }

        public static implicit operator Color4(short d)
        {
            return new Color4() { _RGBA = d };
        }


        public static implicit operator Color4(ValueTuple<byte, byte, byte, byte> rgba)
        {
            var (x, y, z, w) = rgba;
            return new Color4()
            {
                RG = (byte)((rgba.Item2 << 4) | rgba.Item1),
                BA = (byte)((rgba.Item4 << 4) | rgba.Item3)
            };
        }

        public static implicit operator Color4(ValueTuple<byte, byte, byte> rgba)
        {
            var (x, y, z) = rgba;
            return (rgba.Item1, rgba.Item2, rgba.Item3, byte.MaxValue);
        }
    }
}
