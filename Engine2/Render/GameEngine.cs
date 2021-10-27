using Engine2.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Runtime.InteropServices.WindowsRuntime;
using Engine2.Platform;
using System.Collections.Concurrent;
using Engine2.Raytrace;
using System.Threading;

namespace Engine2.Render
{

    class GameEngine
    {
        public double FPS { get; private set; }
        public double dT { get; private set; }
        public double TotalTime { get; private set; }
        public Bitmap_float Image => FrontBuffer;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Camera cam = new Camera(-15, 3, 0);

        public Color4? BackColor { get; set; }
        public ZBuffer ZBuf { get; private set; }

        public Bitmap_float SkyBox = Bitmap_float.FromImage(Properties.Resources.SkyBox);
        public Bitmap_float[] SkyBox_Sides { get; private set; }
        public int SkyBox_Widht;
        public int SkyBox_Height;


        Bitmap_float FrontBuffer;
        Stopwatch frametimer;

        //public Mesh[] monkey = Meshloader.LoadBabylonFile("Horse.babylon");
        public GameObject monkey = Meshloader.LoadBabylonFile("Monkey.babylon")[0];


        public Vector3 LightPos = new Vector3(0, 6, -5);
        public float LightDiameter = 0.5f;

        void MakeSkyBoxes()
        {
            SkyBox_Sides = new Bitmap_float[6];
            int widht = SkyBox.Width / 4;
            int height = SkyBox.Height / 3;
            //https://upload.wikimedia.org/wikipedia/commons/b/b4/Skybox_example.png
            SkyBox_Sides[0] = SkyBox.CutImage(widht, height, widht, height); //left
            SkyBox_Sides[1] = SkyBox.CutImage(widht * 3, height, widht, height); //right

            SkyBox_Sides[3] = SkyBox.CutImage(widht, 0, widht, height); //up
            SkyBox_Sides[2] = SkyBox.CutImage(widht, height*2, widht, height); //down

            SkyBox_Sides[5] = SkyBox.CutImage(widht * 2, height, widht, height); //front
            SkyBox_Sides[4] = SkyBox.CutImage(0, height, widht, height); //back
            SkyBox_Widht = widht;
            SkyBox_Height = height;
        }

        public GameEngine(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            NewImageBuffer();
            MakeSkyBoxes();
        }


        public void Resize(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            NewImageBuffer();
        }

        public ConcurrentQueue<Action> ExecutionQueue = new ConcurrentQueue<Action>();
        public void ExecuteStack()
        {
            Action f;
            while(ExecutionQueue.TryDequeue(out f))
            {
                f();
            }
        }

        void NewImageBuffer()
        {
            FrontBuffer?.Dispose();
            FrontBuffer = new Bitmap_float(Width, Height);
            ZBuf = new ZBuffer(Width, Height);
            frametimer = Stopwatch.StartNew();

            vertices = new VertexRender(FrontBuffer, ZBuf, cam, new GameObject[] { monkey }, this);
            rays = new RayRender(FrontBuffer, ZBuf, cam, new GameObject[] { monkey }, this);
        }


        unsafe void ClearBuffer()
        {
            if (BackColor == null) return;
            Color4 c = BackColor.Value;

            fixed (Color4* buf = FrontBuffer.Pixel)
            {
                Color4* localbuffer = buf;
                float[] distances = ZBuf.distances;
                Parallel.For(0, FrontBuffer.Pixel.Length, (ind) =>
                {
                    distances[ind] = float.PositiveInfinity;
                    localbuffer[ind] = c;
                });
            }
        }




        VertexRender vertices;
        RayRender rays;

        public void DrawAll()
        {
            ClearBuffer();
            if (Properties.Settings.Default.Scene_LightIsCam)
            {
                LightPos = cam.Position - cam.Direction * LightDiameter;
            }
            if(Properties.Settings.Default.Engine_Raytrace)
            {
                rays.Draw();
            }
            else
            {
                vertices.Draw();
            }
        }

        public void DrawLoop(IRenderBitmapHWND Target)
        {
            new Thread(new ThreadStart(() =>
            {
                while(true)
                {
                   
                }
            }))
            { Name = "DrawLoop", IsBackground = true }.Start();
        }



        public void Draw(IRenderBitmapHWND Target)
        {
            FPS = 1 / frametimer.Elapsed.TotalSeconds;
            dT = frametimer.Elapsed.TotalSeconds;
            TotalTime += dT;
            frametimer.Restart();
            DrawAll();
            Target.Draw(FrontBuffer.Pointer, Width, Height, RenderFormat.ARGBFloat);
        }


    }
}
