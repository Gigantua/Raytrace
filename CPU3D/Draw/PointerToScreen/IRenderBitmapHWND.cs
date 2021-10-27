using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public enum RenderFormat
    {
        RGBAFloat,
        RGBA8,
        RGBA4,
        RGB10A2,
    }

    public abstract class IRenderBitmapHWND : IDisposable
    {
        protected IntPtr WindowHandle;
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public IRenderBitmapHWND(IntPtr Target, int width, int height, RenderFormat format)
        {
            this.WindowHandle = Target;
            this.Width = width;
            this.Height = height;
        }

        protected abstract void DrawFrom(IntPtr Source, int width, int height, int BytesPerPixel, bool BilinearDraw);

        protected abstract void resize();

        public void PointerToScreen(IntPtr Source, int width, int heigth, int BytesPerPixel, bool BilinearDraw)
        {
            if (width <= 0 || heigth <= 0) return;
            DrawFrom(Source, width, heigth, BytesPerPixel, BilinearDraw);
        }

        public void Resize(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            this.resize();
        }

        protected abstract void dispose();
        public void Dispose()
        {
            dispose();
        }
    }
}
