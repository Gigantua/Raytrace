using Engine2.Platform;
using Engine2.Primitives;
using Engine2.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Engine2
{
    public partial class Form1 : Form
    {
        GameEngine engine;
        IRenderBitmapHWND imageoutput;
        Keyboard keys;

        new int Width => this.ClientRectangle.Width;
        new int Height => this.ClientRectangle.Height;

        double FPS;

        private void Page_Load(object sender, System.EventArgs e)
        {
            
        }

        private void S_PropertyChanged(object sender, EventArgs e)
        {
            bool DirectX = Properties.Settings.Default.Render_DirectX;
            bool GDI = Properties.Settings.Default.Render_GDI;
            bool Vulcan = Properties.Settings.Default.Render_Vulcan;

            IntPtr HWND = this.Handle;
            engine.ExecutionQueue.Enqueue(() =>
            {
                if (DirectX && !(imageoutput is D2DRender)) { imageoutput.Dispose(); imageoutput = new D2DRender(HWND, Width, Height); }
                if (GDI && !(imageoutput is GDIRender)) { imageoutput.Dispose(); imageoutput = new GDIRender(HWND, Width, Height); }
                if (Vulcan && !(imageoutput is VulkanRender)) { imageoutput.Dispose(); imageoutput = new VulkanRender(HWND, Width, Height); }
            });
        }

        IRenderBitmapHWND GetSetting(IntPtr HWND)
        {
            bool DirectX = Properties.Settings.Default.Render_DirectX;
            bool GDI = Properties.Settings.Default.Render_GDI;
            bool Vulcan = Properties.Settings.Default.Render_Vulcan;

            if (DirectX && !(imageoutput is D2DRender)) { return new D2DRender(HWND, Width, Height); }
            if (GDI && !(imageoutput is GDIRender)) { return new GDIRender(HWND, Width, Height); }
            if (Vulcan && !(imageoutput is VulkanRender)) { return new VulkanRender(HWND, Width, Height); }

            Properties.Settings.Default.Render_DirectX = true;
            Properties.Settings.Default.Save();
            return new D2DRender(HWND, Width, Height);
        }

        public Form1()
        {
            InitializeComponent();
            engine = new GameEngine(Width,Height);
            imageoutput = GetSetting(this.Handle);
            keys = new Keyboard(this);

            Thread gl = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    TimerHandler(null, null);
                };
            })){IsBackground = true, Name = "GameLoop" };
            gl.Start();

            Page_Load(null, null);
            engine.BackColor = Colorconvert.FromColor(System.Drawing.Color.LightBlue);
        }

        bool InTimer = false;
        private void TimerHandler(object source, ElapsedEventArgs e)
        {
            if (InTimer) return;
            InTimer = true;
            FPS = engine.FPS;
            engine.Draw(imageoutput);
            engine.ExecuteStack();

            if (Properties.Settings.Default.Scene_Turn)
                engine.monkey.Rotation = new SharpDX.Vector3(0, engine.monkey.Rotation.Y + (float)engine.dT, engine.monkey.Rotation.Z);

            if (keys.IsDown(Keys.W)) engine.cam.MoveForward((float)engine.dT * 25);
            if (keys.IsDown(Keys.S)) engine.cam.MoveForward(-(float)engine.dT * 25);
            if (keys.IsDown(Keys.A)) engine.cam.MoveRight((float)engine.dT * 25);
            if (keys.IsDown(Keys.D)) engine.cam.MoveRight(-(float)engine.dT * 25);

            InTimer = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = FPS.ToString("0.00") + " " + engine.cam.ToString();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            engine.ExecutionQueue.Enqueue(() =>
            {
                engine.Resize(Width, Height);
                imageoutput?.Resize(Width, Height);
            });
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Cursor.Hide();
            previousLocation = e.Location;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Cursor.Show();
        }

        Point previousLocation = Point.Empty;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(MouseButtons == MouseButtons.Left)
            {
                Point difference = e.Location - (Size)previousLocation;
                engine.cam.Look(difference.X / 180.0f, -difference.Y / 180.0f);

                previousLocation = e.Location;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            var s = new Settings();
            s.PropertyChanged += S_PropertyChanged;
            s.Show();
        }

    }
}
