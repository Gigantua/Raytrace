using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CPU3D.RayTrace;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public class Canvas : CanvasBase<Color, Vector3,float>
    {
        public Canvas(int width, int height) : base(width, height)
        {

        }

        public override CanvasBase<Color, Vector3, float> Copy()
        {
            Canvas cps = new Canvas(Width, Height);
            Array.Copy(this.Pixel, 0, cps.Pixel, 0, this.Pixel.Length);
            return cps;
        }
    }
}
