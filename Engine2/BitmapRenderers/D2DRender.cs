using Engine2.Render;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Platform
{

    class D2DRender : IRenderBitmapHWND
    {
        WindowRenderTarget wndRender = null;
        Factory fact2d = new Factory(FactoryType.SingleThreaded);
        RenderTargetProperties rndTargProperties = new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Ignore));
        
        Bitmap dxbitmap;
        BitmapProperties bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R32G32B32A32_Float, SharpDX.Direct2D1.AlphaMode.Ignore));

        public D2DRender(IntPtr ControlHandle,int WindowWidth,int WindowHeight) : base(ControlHandle, WindowWidth, WindowHeight)
        {
            HwndRenderTargetProperties hwndProperties = new HwndRenderTargetProperties();
            hwndProperties.Hwnd = ControlHandle;
            hwndProperties.PixelSize = new Size2(WindowWidth, WindowHeight);
            hwndProperties.PresentOptions = PresentOptions.Immediately; //no vsync
            wndRender = new WindowRenderTarget(fact2d, rndTargProperties, hwndProperties);
        }


        void CreateBitmap(IntPtr Source,int width,int height)
        {
            if (dxbitmap != null) dxbitmap.Dispose();
            dxbitmap = new Bitmap(wndRender, new Size2(Width, Height), new DataPointer(Source, width * height * 16), width * 16, bitmapProperties);
        }

        protected override void resize()
        {
            lock(wndRender)
            {
                wndRender.Resize(new Size2(Width, Height));
            }
        }

        protected override void dispose()
        {
            dxbitmap?.Dispose();
            wndRender?.Dispose();
            fact2d?.Dispose();
        }

        IntPtr LastSource = IntPtr.Zero;
        protected override void DrawFrom(IntPtr Source, int width, int height, RenderFormat format)
        {
            int stride = width * 4 * sizeof(float);
            if (LastSource!=Source)
            {
                CreateBitmap(Source,width,height);
                Source = LastSource;
            }
            
            wndRender.BeginDraw();

            wndRender.DrawBitmap(dxbitmap, 1, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            wndRender.Flush();
            wndRender.EndDraw();
        }
    }
}
