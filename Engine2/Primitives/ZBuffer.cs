using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine2.Render;
using System.Threading;

namespace Engine2.Primitives
{
    class ZBuffer
    {
        public readonly float[] distances;
        readonly int width;
        readonly int height;

        public ZBuffer(int width,int height)
        {
            distances = new float[width * height];
            this.width = width;
            this.height = height;
        }


        public void Reset()
        {
            Parallel.For(0, distances.Length, (i) =>
             {
                 distances[i] = float.PositiveInfinity;
             });
        }

        static bool InterlockedExchangeIfSmaller(ref float location, float comparison, float newValue)
        {
            float initialValue;
            do
            {
                initialValue = location;
                if (comparison > initialValue) return false;
            }
            while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);
            return true;
        }

        public void DrawPoint(int x, int y, float z, Color4 color, Bitmap_float frontBuffer)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;

            int index = width * y + x;
            

            if (InterlockedExchangeIfSmaller(ref distances[index], z, z))
            {
                frontBuffer[index] = color;
            }
        }
    }
}
