using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    class Canvas10 : CanvasBase<Color10, int, ushort>
    {
        public Canvas10(int width, int height) : base(width, height)
        {

        }

        public override CanvasBase<Color10, int, ushort> Copy()
        {
            Canvas10 cps = new Canvas10(Width, Height);
            Array.Copy(this.Pixel, 0, cps.Pixel, 0, this.Pixel.Length);
            return cps;
        }

    }
}
