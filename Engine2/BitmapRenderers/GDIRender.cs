using Engine2.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Platform
{
    class GDIRender : IRenderBitmapHWND
    {
        Graphics g;
        Bitmap bmp;
        IntPtr Scan0;

        public GDIRender(IntPtr Target, int width, int height) : base(Target, width, height)
        {
            Init();
        }

        void Init()
        {
            Dispose();

            g = Graphics.FromHwnd(base.WindowHandle);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            Scan0 = Marshal.AllocHGlobal(Width * Height * 4);
            bmp = new Bitmap(Width, Height, Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, Scan0);
        }

        
        protected override void dispose()
        {
            g?.Dispose();
            if (Scan0 != IntPtr.Zero) Marshal.FreeHGlobal(Scan0);
            bmp?.Dispose();
        }

        protected override void DrawFrom(IntPtr Source, int width, int height, RenderFormat format)
        {
            BitmapConverter.To_BGRA32(Source,Scan0, width, height);
            g.DrawImage(bmp, new Rectangle(0, 0, this.Width, this.Height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
        }

        protected override void resize()
        {
            Init();
        }
    }
}
