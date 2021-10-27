using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace RayForm
{
    public enum PixelFormat
    {
        RGBA8_Byte,
        RGBA32_Float
    }

    public class DirectDraw
    {
        WindowRenderTarget wndRender = null;
        Factory fact2d = new Factory(FactoryType.SingleThreaded);
        RenderTargetProperties rndTargProperties = new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Ignore));

        Bitmap1 Bitmap;
        BitmapProperties1 BitmapProperties;

        public DirectDraw(IntPtr ControlHandle, int WindowWidth, int WindowHeight, PixelFormat format)
        {
            var pxfrmt = format == PixelFormat.RGBA32_Float ? SharpDX.DXGI.Format.R32G32B32A32_Float : SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            BitmapProperties = new BitmapProperties1(new SharpDX.Direct2D1.PixelFormat(pxfrmt, SharpDX.Direct2D1.AlphaMode.Ignore));

            HwndRenderTargetProperties hwndProperties = new HwndRenderTargetProperties();
            hwndProperties.Hwnd = ControlHandle;
            hwndProperties.PixelSize = new SharpDX.Size2(WindowWidth, WindowHeight);
            hwndProperties.PresentOptions = PresentOptions.Immediately; //no vsync
            rndTargProperties.Type = RenderTargetType.Hardware;

            wndRender = new WindowRenderTarget(fact2d, rndTargProperties, hwndProperties);
            this.WindowWidth = WindowWidth;
            this.WindowHeight = WindowHeight;
            
            textBrush = new SolidColorBrush(wndRender.QueryInterface<DeviceContext>(), new RawColor4(1f, 0.9f, 0.329f, 1f));
        }

        void CreateBitmap(IntPtr Source, int canvaswidth, int canvasheight, int BytesPerPixel)
        {
            if (Bitmap != null) Bitmap.Dispose();
            var deviceContext2d = wndRender.QueryInterface<DeviceContext>();

            Bitmap = new Bitmap1(deviceContext2d, new Size2(canvaswidth, canvasheight), BitmapProperties);
        }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public void Resize(int WindowWidth, int WindowHeight)
        {
            wndRender.Resize(new Size2(WindowWidth, WindowHeight));
            this.WindowWidth = WindowWidth;
            this.WindowHeight = WindowHeight;
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

        SharpDX.DirectWrite.TextFormat format = new SharpDX.DirectWrite.TextFormat(new SharpDX.DirectWrite.Factory(), "Calibri", 48.0f);
        SolidColorBrush textBrush;

        public void Draw(IntPtr Source, int BmpWidth, int BmpHeight,string text, int BytesPerPixel, bool BilinearDraw)
        {
            if (BmpWidth < 1 || BmpHeight < 1) return;
            BitmapInterpolationMode mode = BilinearDraw ? BitmapInterpolationMode.Linear : BitmapInterpolationMode.NearestNeighbor;

            SetBmp(Source, BmpWidth, BmpHeight, BytesPerPixel);
            wndRender.BeginDraw();
            wndRender.DrawBitmap(Bitmap, 1, mode);//new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, WindowWidth, WindowHeight), 
            wndRender.DrawText(text, format, new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, WindowWidth / 2.0f, WindowHeight / 16.0f), textBrush);
            wndRender.EndDraw();
        }

    }
}
