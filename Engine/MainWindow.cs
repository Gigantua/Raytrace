using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using SharpDX.Direct3D;
using SharpDX.Direct2D1;
using System.Text;
using System.Windows.Forms;
using SharpDX.DXGI;
using SharpDX;
using System.Globalization;
using System.Diagnostics;
using Engine.UI;
using Engine.Drawable;

namespace Engine
{
    public partial class MainWindow : Form
    {
        Stopwatch time = Stopwatch.StartNew();
        Renderer render;
        KeyboardWatcher keyboard;
        PreciseTimer timer;
        Stopwatch totaltime = Stopwatch.StartNew();
        Image2D subnautica = Image2D.FromFile(@"Subnautica.jpg");
        Random r = new Random();

        WindowRenderTarget wndRender = null;
        SharpDX.Direct2D1.Factory fact2d = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded);
        RenderTargetProperties rndTargProperties = new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore));
        HwndRenderTargetProperties hwndProperties;
        SharpDX.Direct2D1.Bitmap dxbitmap;

        public MainWindow()
        {
            InitializeComponent();
            keyboard = new KeyboardWatcher(this);

            render = new Renderer(this.ClientRectangle.Width, this.ClientRectangle.Height);
            render.GameObjects.Add(subnautica);
            renderpixel = this.ClientRectangle.Width * this.ClientRectangle.Height;

            var screenfps = Engine.Hardware.Screen.QueryCurrentSettings();
            timer = PreciseTimer.FromFPS(1000);
           
            this.MouseDown += PictureBox1_MouseDown;
            this.DoubleBuffered = true;
            Init2D();
        }

        unsafe private SharpDX.Direct2D1.Bitmap CreateBitmap(System.Drawing.Bitmap bitmap)
        {
            int stride = bitmap.Width * sizeof(int);

            var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Ignore));
            return new SharpDX.Direct2D1.Bitmap(wndRender, new Size2(bitmap.Width, bitmap.Height), new DataPointer(render.buffer.Pointer,render.buffer.PointerSize) , stride, bitmapProperties);
        }

        void Init2D()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            hwndProperties = new HwndRenderTargetProperties();
            hwndProperties.Hwnd = this.Handle;
            hwndProperties.PixelSize = new Size2(this.ClientRectangle.Width, this.ClientRectangle.Height);
            hwndProperties.PresentOptions = PresentOptions.Immediately; //vsync
            wndRender = new WindowRenderTarget(fact2d, rndTargProperties, hwndProperties);
            
            var dpi = fact2d.DesktopDpi;

            dxbitmap = CreateBitmap(render.buffer.Bitmap);
            render.Draw2D();
        }


        void DrawBufferImage()
        {
            dxbitmap.CopyFromMemory(render.buffer.Pointer, render.buffer.Bitmap.Width * sizeof(int));

            wndRender.BeginDraw();

            wndRender.DrawBitmap(dxbitmap, 1, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            wndRender.Flush();
            wndRender.EndDraw();
        }


        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            subnautica.Location = new Point2D(e.X, e.Y);
        }


        private void Frametimer_Tick(object sender, TimeSpan e)
        {
            this.Invoke(new Action(() =>
            {
                double mpps = renderpixel / (timer.Average.TotalSeconds*1000000.0);
                this.Text = (timer.Average.TotalMilliseconds).ToString("0.00") + " ms - " + mpps.ToString("0.0") + " mpps";
            }));
            lock (render)
            {
                subnautica.RotationCenter = new Point2D(100, 100);
                subnautica.Alpha = 0.5;

                render.BackColor = -1;

                render.Draw2D();
                render.WaitRenderComplete();

                DrawBufferImage();
            }
        }

        int renderpixel;

        private void Form1_Resize(object sender, EventArgs e)
        {
            lock (render)
            {
                render.Resize(this.ClientRectangle.Width, this.ClientRectangle.Height);
                if (dxbitmap != null) dxbitmap.Dispose();
                dxbitmap = CreateBitmap(render.buffer.Bitmap);

                renderpixel = this.ClientRectangle.Width * this.ClientRectangle.Height;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.TimerElapsedEvent += Frametimer_Tick;
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
    }
}
