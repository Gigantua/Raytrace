using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    class D2DRender : IRenderBitmapHWND
    {
        WindowRenderTarget wndRender = null;
        Factory fact2d = new Factory(FactoryType.SingleThreaded);
        RenderTargetProperties rndTargProperties = new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Ignore));

        Bitmap1 Bitmap;
        BitmapProperties1 BitmapProperties;

        public D2DRender(IntPtr ControlHandle, int WindowWidth, int WindowHeight, RenderFormat format) : base(ControlHandle, WindowWidth, WindowHeight, format)
        {
            switch(format)
            {
                case RenderFormat.RGBAFloat:
                    BitmapProperties = new BitmapProperties1(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R32G32B32A32_Float, SharpDX.Direct2D1.AlphaMode.Ignore));
                    break;
                case RenderFormat.RGBA8:
                    BitmapProperties = new BitmapProperties1(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore));
                    break;
                case RenderFormat.RGBA4:
                    BitmapProperties = new BitmapProperties1(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B4G4R4A4_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore));
                    break;
                case RenderFormat.RGB10A2:
                    BitmapProperties = new BitmapProperties1(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R10G10B10A2_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore));
                    break;
                default: throw new NotImplementedException("Pixel format " + format + " not supported");
            }

            HwndRenderTargetProperties hwndProperties = new HwndRenderTargetProperties();
            hwndProperties.Hwnd = ControlHandle;
            hwndProperties.PixelSize = new Size2(WindowWidth, WindowHeight);
            hwndProperties.PresentOptions = PresentOptions.Immediately; //no vsync

            wndRender = new WindowRenderTarget(fact2d, rndTargProperties, hwndProperties);

        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        void CreateBitmap(IntPtr Source, int canvaswidth,int canvasheight, int BytesPerPixel)
        {
            if (Bitmap != null) Bitmap.Dispose();

            var deviceContext2d = wndRender.QueryInterface<DeviceContext>();

            int size = deviceContext2d.MaximumBitmapSize;

            Bitmap = new Bitmap1(deviceContext2d, new Size2(canvaswidth, canvasheight), BitmapProperties);
        }

        protected override void resize()
        {
            lock (wndRender)
            {
                wndRender.Resize(new Size2(Width, Height));
            }
        }

        protected override void dispose()
        {
            Bitmap?.Dispose();
            wndRender?.Dispose();
            fact2d?.Dispose();
        }

        IntPtr LastSource = IntPtr.Zero;

        void SetBmp(IntPtr Source, int width, int height, int BytesPerPixel)
        {
            int pitch = width * BytesPerPixel;
            if (LastSource != Source)
            {
                CreateBitmap(Source, width, height, BytesPerPixel);
                LastSource = Source;
            }
            else
            {
                Bitmap.CopyFromMemory(Source, pitch);
            }
        }

        protected override void DrawFrom(IntPtr Source, int width, int height, int BytesPerPixel, bool BilinearDraw)
        {
            lock (wndRender)
            {
                BitmapInterpolationMode mode = BilinearDraw ? BitmapInterpolationMode.Linear : BitmapInterpolationMode.NearestNeighbor;

                SetBmp(Source, width, height, BytesPerPixel);
                wndRender.BeginDraw();
                wndRender.DrawBitmap(Bitmap, new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, Width, Height), 1, mode);
                //wndRender.DrawLine(new SharpDX.Mathematics.Interop.RawVector2(0, 0), new SharpDX.Mathematics.Interop.RawVector2(100, 100), new SolidColorBrush(wndRender, new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1)));
                wndRender.Flush();
                wndRender.EndDraw();
            }
        }

    }
}
