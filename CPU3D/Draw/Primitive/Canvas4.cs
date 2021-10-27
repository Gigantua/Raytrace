using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public class Canvas4 : CanvasBase<Color4, short, byte>
    {
        public Canvas4(int width, int height) : base(width, height)
        {

        }

        public override CanvasBase<Color4, short, byte> Copy()
        {
            Canvas4 cps = new Canvas4(Width, Height);
            Array.Copy(this.Pixel, 0, cps.Pixel, 0, this.Pixel.Length);
            return cps;
        }
    }
}
