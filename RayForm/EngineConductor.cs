using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayForm
{
    public class EngineConductor
    {
        CancellationTokenSource CancelSource  = new CancellationTokenSource();
        DirectDraw Renderer;
        Thread Drawing;

        FrameBuffer buffer;
        Scene scene;

        public int Width { get; }
        public int Height { get; }

        public EngineConductor(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            buffer = new FrameBuffer(Width, Height);//buffer8
            scene = new Scene();
        }

        HashSet<Keys> keys = new HashSet<Keys>();

        public void Run()
        {
            MainForm window = new MainForm(Width, Height);
            window.Shown += Window_Shown;
            window.FormClosing += (sender, e) => CancelSource.Cancel();
            window.KeyDown += (sender, e) => keys.Add(e.KeyCode);
            window.KeyUp += (sender, e) => keys.Remove(e.KeyCode);
            window.LostFocus += (sender, e) => keys.Clear();
            window.MouseDragged += Window_MouseDragged;
            Application.Run(window);
        }

        int mousex = 0; int mousey = 0;
        private void Window_MouseDragged(object sender, (int dx, int dy) e)
        {
            mousex += e.dx; mousey += e.dy;
        }

        Stopwatch dT = Stopwatch.StartNew();
        List<double> fps = new List<double>();
        int fpsptr = 0;

        public double FPS
        {
            get
            {
                if (fps.Count == 0) return 0;
                return fps.Average();
            }
            set
            {
                if (fps.Count < 16) fps.Add(value);
                else fps[fpsptr++] = value;
                if (fpsptr == 16) fpsptr = 0;
            }
        }
        public void HandleInput()
        {
            float t = (float)dT.Elapsed.TotalSeconds * 4;
            if (keys.Contains(Keys.W)) scene.cam.MoveForward(t);
            if (keys.Contains(Keys.S)) scene.cam.MoveForward(-t);
            if (keys.Contains(Keys.A)) scene.cam.MoveRight(-t);
            if (keys.Contains(Keys.D)) scene.cam.MoveRight(t);

            if (mousex != 0) scene.cam.RotateLeft(mousex / 100.0);
            if (mousey != 0) scene.cam.RotateUp(mousey / 100.0);

            mousex = 0; mousey = 0;
        }


        void DrawLoop()
        {
            Random r = new Random();
            scene.Prepare(buffer.Width, buffer.Height);
            while(!CancelSource.IsCancellationRequested)
            {
                scene.Draw(buffer);
                Renderer.Draw(buffer.Pointer, Width, Height, FPS.ToString("0.00") , buffer.BytesPerPixel, true);
                HandleInput();
                FPS = 1 / dT.Elapsed.TotalSeconds; dT.Restart();
            }
        }

       

        private void Window_Shown(object sender, EventArgs e)
        {
            Renderer = (sender as MainForm).Render;
            Drawing = new Thread(DrawLoop)
            {
                Priority = ThreadPriority.Highest,
                Name = "DrawLoop",
            };
            Drawing.Start();
        }
    }
}
