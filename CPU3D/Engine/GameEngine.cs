using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPU3D.Draw;
using CPU3D.Logic;
using CPU3D.Objects;
using CPU3D.Platform;
using CPU3D.Platform.Windows;
using CPU3D.RayTrace;
using System.Threading;

namespace CPU3D.Engine
{
    /// <summary>
    ///HandleUserInput();
    ///ChangeWorld();
    ///Render();
    /// </summary>
    class GameEngine
    {
        Form DrawTarget;
        public Keyboard keys;
        public Mouse mouse;
        public Objects.Scene scene = new Objects.Scene();
        public bool BilinearDraw { get; set; } = true;


        TimeMeasure FrameTimer = new TimeMeasure("Frame");

        D2DRender renderer;
        Canvas8 worldview;
        MultimediaTimer timer;
        InvokeTimer UIInvoke = new InvokeTimer(TimeSpan.FromMilliseconds(10));
        Stopwatch dTs;
        Stopwatch T;
        float TargetFPS = 144;

        AverageList frametimes = new AverageList(30);
        Video v;

        public event EventHandler<Canvas8> FrameBeforePaint;
        public event EventHandler<Canvas8> FrameAfterPaint;

        public event EventHandler MouseUnlocked;
        public event EventHandler MouseLocked;

        public double ElapsedSeconds => T.Elapsed.TotalSeconds;

        public Canvas8 Canvas => worldview;


        System.Drawing.Rectangle Viewpos;


        public void SaveImage(string path)
        {
            lock (timer.ElapseLock)
            {
            
                Canvas8 tmp = worldview;
                worldview = new Canvas8(3840, 2160);
                var frame = new Video(path, worldview.Width, worldview.Height, Codec.Bitmaps);
                Scene.MaxDepth = 50;
                DrawWorld();
                DrawWorld();

                (System.Drawing.Bitmap bmp, IntPtr ptr) = frame.BitmapFromPointer(worldview.Pointer);
                bmp.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
                bmp.Dispose();
                Marshal.FreeHGlobal(ptr);

                worldview = tmp;
            }
        }

        public void StartRecord(string fileName)
        {
            if (fileName.EndsWith(".mkv"))
            {
                v = new Video(fileName, worldview.Width, worldview.Height, Codec.H264);
            }
            else
            {
                v = new Video(fileName, worldview.Width, worldview.Height, Codec.Bitmaps);
            }
        }

        public bool IsRecording => v != null;

        public void StopRecord()
        {
            if (v != null) { v.Save(); v = null; }
        }


        public GameEngine(Form WindowsControl)
        {
            this.DrawTarget = WindowsControl;
            WindowsControl.KeyPreview = true;
            keys = new Keyboard(WindowsControl);
            mouse = new Mouse(WindowsControl);
            mouse.Locked += (sender, e) => MouseLocked(sender, e);
            mouse.Unlocked += (sender, e) => MouseUnlocked(sender, e);

            renderer = new D2DRender(WindowsControl.Handle, WindowsControl.ClientRectangle.Width, WindowsControl.ClientRectangle.Height, RenderFormat.RGBA8);
            WindowsControl.SizeChanged += (form, e) => { renderer.Resize(WindowsControl.ClientRectangle.Width, WindowsControl.ClientRectangle.Height); };
            worldview = new Canvas8(1024/2, 768/2); //Must be power of two
            dTs = Stopwatch.StartNew();
            T = Stopwatch.StartNew();
            
            worldview.Draw.PixelInvoke((xy) => Color8.Red);
            WindowsControl.FormClosed += (sender, e) =>
            {
                timer?.Stop();
                if (v != null) v.Save();
            };
            Viewpos = DrawTarget.RectangleToScreen(DrawTarget.ClientRectangle);
            DrawTarget.SizeChanged += (sender, e) => Viewpos = DrawTarget.RectangleToScreen(DrawTarget.ClientRectangle);
            DrawTarget.LocationChanged += (sender, e) => Viewpos = DrawTarget.RectangleToScreen(DrawTarget.ClientRectangle);
            WindowsControl.Shown += WindowsControl_Shown;

            scene.AddObject(new Objects.Disk(Vector3.Zero, Vector3.UnitY, 0, 250)
            {
                Material = Material.ColorIsFunction(Material.CheckerBoardFunction)
            });
            
            var bluesphere = new Objects.Sphere(Vector3.UnitY * 40, 35) { Material = Material.Realistic((0, 0, 0.7f), 1.52f) };
            bluesphere.Material.HasShadow = false;
            bluesphere.Material.Optics_Absorptionrate = 0.07f;
            bluesphere.Material.Optics_RefractionIndex = 1.7f;
            scene.AddObject(bluesphere);


            scene.AddObject(new Objects.Sphere(Vector3.UnitY * 20 + Vector3.UnitX * 65, 20)
            {
                Material = Material.Reflect((192 / 255.0f, 192 / 255.0f, 192 / 255.0f), 0.7f),
            });

            /*
            for (int x = -800; x < 800; x += 80)
            {
                for (int y = -800; y < 800; y += 80)
                {
                    //for (int z = -400; z < 400; z += 80)
                    //{
                    Vector3 dr3 = new Vector3(x, y, 80);

                    scene.AddObject(new Objects.Box(Vector3.UnitZ * 45 + Vector3.UnitY * 25f + dr3, Vector3.One * 35 + Vector3.UnitZ * 45 + Vector3.UnitY * 25f + dr3)
                    {
                        Material = Material.Simple(Colors.White),
                    });
                    //}
                }
            }
            */


            Vector3 dr = new Vector3(0, 0, 0);
            scene.AddObject(new Objects.Box(Vector3.UnitZ * 45 + Vector3.UnitY * 25f + dr, Vector3.One * 35 + Vector3.UnitZ * 45 + Vector3.UnitY * 25f + dr)
            {
                Material = Material.Realistic((0.85f, 0.2f, 0.2f), 1.33f)
            });
            scene.Objects.Last().Material.Optics_Absorptionrate = 0.04f;


            scene.AddObject(new Cylinder(new Vector3(60,50,50), new Vector3(60, 120, 50), 15)).Material = Material.Realistic((0.85f, 0.85f, 0.05f), 1.22f);
            scene.Objects.Last().Material.Optics_Absorptionrate = 0.0f;
            scene.Objects.Last().Material.HasShadow = false;

            scene.cam.Position = new Vector3(105, 50, 65);

            /*
            for (int x = -1800; x < -1000; x += 80)
            {
                for (int y = -1800; y < -1000; y += 80)
                {
                    Vector3 dr3 = new Vector3(x, 10, y);
                    Vector3 dr4 = new Vector3(x, (float)(11 + 500 * Colors.Rand.NextDouble()), y);

                    scene.AddObject(new Cylinder(dr3, dr4, 15)).Material = Material.Realistic(Colors.Random, (float)(1+Colors.Rand.NextDouble()));
                    scene.Objects.Last().Material.Optics_Absorptionrate = 0.08f;
                    scene.Objects.Last().Material.HasShadow = false;
                }
            }
            */

            timer = new MultimediaTimer(1000 / (int)TargetFPS);
            timer.Elapsed += (sender, e) => Timer_Elapsed(T.Elapsed.TotalSeconds);
        }



        private void WindowsControl_Shown(object sender, EventArgs e)
        {
            mouse.Lock();
            DrawTarget.Invoke(new Action(timer.Start));
        }

        bool CastMouseray(out HitInfo info)
        {
            Vector2 wh = new Vector2(worldview.Width, worldview.Height);
            Vector2 screenpos = mouse.GetPixelXY(new Vector2(Viewpos.X, Viewpos.Y), new Vector2(Viewpos.Width, Viewpos.Height), wh);
            Ray r = Ray.CastScreen(screenpos, wh, scene.cam);
            return scene.CastSceneRay(r, out info);
        }


        void UserInput()
        {
            float camspeed = 250f;
            if (keys.IsDown(Keys.ControlKey, out float dtc)) camspeed = 50f;

            if (keys.IsDown(Keys.W, out float dTW)) scene.cam.MoveForward(camspeed * dTW); //dT time is sub frame keypress. -> No overshoot if frame take 1 s
            if (keys.IsDown(Keys.S, out float dTS)) scene.cam.MoveForward(-camspeed * dTS);
            if (keys.IsDown(Keys.A, out float dTA)) scene.cam.Strave(-camspeed * dTA);
            if (keys.IsDown(Keys.D, out float dTD)) scene.cam.Strave(camspeed * dTD);
            if (keys.IsDown(Keys.Q, out float dTQ)) scene.cam.Rotate(2.0f * dTQ);
            if (keys.IsDown(Keys.E, out float dTE)) scene.cam.Rotate(-2.0f * dTE);

            if (keys.ChangedDown(Keys.Escape))
            {
                DrawTarget.Invoke(new Action(mouse.Unlock));
            }
            if (keys.IsDown(Keys.Space, out float dst))
            {
                //var sphere = new Sphere(scene.cam.Position + scene.cam.Direction * 2, 20);
                //sphere.Query = RenderQuery.Color((1, 0, 0));
                //scene.AddObject(sphere);

                scene.Lights[0].Position = scene.cam.Position;
            }
            if (mouse.ChangedDown(MouseButtons.Right))
            {
                if (!mouse.locked) //double rightlick = open properties dialogue
                {
                    if (CastMouseray(out var info))
                    {
                        new ObjectProperties(info.HitObj).ShowSingle();
                    }
                }
                DrawTarget.Invoke(new Action(mouse.Unlock));
            }
            if (mouse.ChangedDown(MouseButtons.Left))
            {
                DrawTarget.Invoke(new Action(mouse.Lock));
            }

            scene.ClearHover();
            if (!mouse.locked && !ObjectProperties.DialogueOpen)//Select an object
            {
                if (CastMouseray(out HitInfo info))
                {
                    info.HitObj.Material.IsMouseHover = true;
                }
            }

            scene.cam.RotateScreen(mouse.dX / 180.0f, mouse.dY / 180.0f);

            keys.Frame();
            mouse.Frame();
        }

        void WorldChange(float dT)
        {
            if (scene.cam.Position.Y < 10f)
            {
                scene.cam.Position = new Vector3(scene.cam.Position.X, 10f, scene.cam.Position.Z);
            }

            foreach (var item in scene.Objects)
            {
                item.physics.UpdateForces(dT);
                item.physics.Animate(dT);
            }
        }

        void FXAA()
        {
            worldview.Draw.Neighbourinvoke((up, left, right, bottom, center) =>
            {
                double lumaUp = up.Luma();
                double lumaLeft = left.Luma();
                double lumaRight = right.Luma();
                double lumaDown = bottom.Luma();
                double lumaCenter = center.Luma();

                double lumaMin = Math.Min(lumaCenter, Math.Min(Math.Min(lumaDown, lumaUp), Math.Min(lumaLeft, lumaRight)));
                double lumaMax = Math.Max(lumaCenter, Math.Max(Math.Max(lumaDown, lumaUp), Math.Max(lumaLeft, lumaRight)));

                double lumaRange = lumaMax - lumaMin;

                double EDGE_THRESHOLD_MIN = 0.0312*0;
                double EDGE_THRESHOLD_MAX = 0.125*8;
                if (lumaRange < Math.Max(EDGE_THRESHOLD_MIN, lumaMax * EDGE_THRESHOLD_MAX))
                {
                    return center;
                }

                return Color8.Average(up, left, right, bottom, center);
            });
        }

        bool checker;
        //26.62;
        void DrawWorld()
        {
            Camera cam = scene.cam; //Make vars lokal for speed
            Scene s = this.scene;
            ratio = (float)worldview.Width / worldview.Height;

            pos = cam.Position;
            dir = cam.Direction;
            right = cam.Right;
            up = cam.Up;

            scene.FrameStart();
            //t.Run("Octree Build", (x) => scene.FrameStart());

            Vector2 wh = new Vector2(worldview.Width, worldview.Height);
            Vector2 zoom = cam.HalfTanFOV2;
            worldview.Draw.GetUVCheckerBoard(ref checker, wh, ratio, zoom, SceneCast);
        }

        static float ratio;
        static Vector3 pos;
        static Vector3 dir;
        static Vector3 right;
        static Vector3 up;

        bool SceneCast(Vector2 xy, out Color8 c)
        {
            Ray r = Ray.CastUV(ratio, xy, pos, dir, right, up);
            Color castray = scene.CastRay(r, out var info);
            // if (castray.IsNan) return (true, Color8.Green);

            c = Colors.Convert8Bit(castray);
            return true;
        }

        void Render()
        {
            FrameBeforePaint?.Invoke(this, worldview);
            DrawWorld();

            renderer.PointerToScreen(worldview.Pointer, worldview.Width, worldview.Height, worldview.BytesPerPixel, BilinearDraw);
            //t.Run(nameof(DrawWorld), (x) => DrawWorld(x));
            
            //t.Run("FXAA", (x) => FXAA());

            //renderer.PointerToScreen(tex.Pixel.Pointer, tex.Pixel.Width, tex.Pixel.Height, 3);
            //t.Run("DirectDraw", (x) => renderer.PointerToScreen(worldview.Pointer, worldview.Width, worldview.Height, worldview.BytesPerPixel, BilinearDraw));
            FrameAfterPaint?.Invoke(this, worldview);
        }

        private void Timer_Elapsed(double Time)
        {
            float dT = (float)dTs.Elapsed.TotalSeconds;
            dTs.Restart();

            FrameTimer.Clear();

            scene.RayCount = 0;
            UserInput();
            WorldChange(dT);
            Render();

            if (v != null) FrameTimer.Run("Video Generation", (x) => v.WriteFrame(worldview.Pointer));

            frametimes.Add(dTs.Elapsed.TotalMilliseconds);
            string camstr = scene.cam.ToString();

            UIInvoke.AsyncInvoke(() => DrawTarget.Text = $"Frametime={frametimes.Average.ToString("0.00")} ms Rays={scene.RayCount}", DrawTarget);
        }
    }
}
