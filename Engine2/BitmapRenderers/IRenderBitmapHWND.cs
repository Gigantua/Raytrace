using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Render
{
    public enum RenderFormat
    {
        ARGBFloat,
    }

    public abstract class IRenderBitmapHWND : IDisposable
    {
        protected IntPtr WindowHandle;
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public IRenderBitmapHWND(IntPtr Target, int width, int height)
        {
            this.WindowHandle = Target;
            this.Width = width;
            this.Height = height;
        }

        protected abstract void DrawFrom(IntPtr Source, int width, int height, RenderFormat format);

        protected abstract void resize();

        public void Draw(IntPtr Source,int width,int heigth,RenderFormat format)
        {
            if (width <= 0 || heigth <= 0) return;
            DrawFrom(Source, width, heigth, format);
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
