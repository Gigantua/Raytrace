using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public class Canvas8 : CanvasBase<Color8, int, byte>
    {
        public Canvas8(int width, int height) : base(width, height)
        {
            bmp = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, Pointer);
            G = Graphics.FromImage(bmp);
        }

        public Bitmap bmp;
        public Graphics G;

        public override CanvasBase<Color8, int, byte> Copy()
        {
            Canvas8 cps = new Canvas8(Width, Height);
            Array.Copy(this.Pixel, 0, cps.Pixel, 0, this.Pixel.Length);
            return cps;
        }
    }
}
