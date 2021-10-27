using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [StructLayout(LayoutKind.Explicit)]
    public struct NativeColor
    {
        [FieldOffset(0)]
        public int RGBA;
        [FieldOffset(0)]
        public byte R;
        [FieldOffset(1)]
        public byte G;
        [FieldOffset(2)]
        public byte B;
        [FieldOffset(3)]
        public byte A;

        public static implicit operator NativeColor(int color)
        {
            return new NativeColor() { RGBA = color };
        }

        /// <summary>
        /// R,G,B,A 0 to 255. 255 = max
        /// </summary>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// <param name="A"></param>
        public NativeColor(byte R, byte G, byte B, byte A)
        {
            RGBA = 0;
            this.R = (byte)(R);
            this.G = (byte)(G);
            this.B = (byte)(B);
            this.A = (byte)(A);
        }

        public static implicit operator NativeColor(System.Drawing.Color color)
        {
            return new NativeColor() { RGBA = color.ToArgb() };
        }

        public static NativeColor operator +(NativeColor backColor, NativeColor color)
        {
            double amount = (1.0 * color.A) / byte.MaxValue;
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return new NativeColor(r, g, b, 255);
        }

        /*
        public static int BlendColor(int A, int B)
        {
            byte[] Abytes = BitConverter.GetBytes(A);
            byte[] Bbytes = BitConverter.GetBytes(B);

            int rOut = (Abytes[0] * Abytes[3] / 255) + (Bbytes[0] * Bbytes[3] * (255 - Abytes[3]) / (255 * 255));
            int gOut = (Abytes[1] * Abytes[3] / 255) + (Bbytes[1] * Bbytes[3] * (255 - Abytes[3]) / (255 * 255));
            int bOut = (Abytes[2] * Abytes[3] / 255) + (Bbytes[2] * Bbytes[3] * (255 - Abytes[3]) / (255 * 255));
            int aOut = Abytes[3] + (Bbytes[3] * (255 - Abytes[3]) / 255);

            byte[] Target = new byte[4];
            Target[0] = (byte)(rOut);
            Target[1] = (byte)(gOut);
            Target[2] = (byte)(bOut);
            Target[3] = (byte)(aOut);
            return BitConverter.ToInt32(Target, 0);
        }
        */
    }
}
