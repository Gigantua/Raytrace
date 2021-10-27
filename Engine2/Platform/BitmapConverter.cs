using Engine2.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Platform
{
    static class BitmapConverter
    {


        public static byte[] To_RGBA32(IntPtr SourceBitmap, int Width, int Height)
        {
            /*
            float[] Data = GetSource(SourceBitmap, Width, Height);
            byte[] Outputs = new byte[Data.Length];

            Parallel.For(0, Data.Length, (i) =>
            {
                Outputs[i] = (byte)(Data[i] * 255.0f);
            });

            return Outputs;
            */
            return null;
        }

        public static unsafe void To_BGRA32(IntPtr SourceBitmap,IntPtr TargetBitmap, int Width, int Height)
        {
            int elements = Width * Height * 4;

            byte* target = (byte*)TargetBitmap;
            float* source = (float*)SourceBitmap;

            Parallel.For(0, elements / 4, (ind) =>
            {
                int i = ind * 4;

                //BGRA
                target[i + 2] = (byte)(source[i + 0] * byte.MaxValue);
                target[i + 1] = (byte)(source[i + 1] * byte.MaxValue);
                target[i + 0] = (byte)(source[i + 2] * byte.MaxValue);
                target[i + 3] = (byte)(source[i + 3] * byte.MaxValue);
            });

        }

    }
}
