using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CPU3D.RayTrace;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU3D.Engine
{
    class Mouse : Inputdevice
    {
        Form Source;
        public bool locked = false;
        public event EventHandler Locked;
        public event EventHandler Unlocked;

        public void Lock()
        {
            if (locked) return;
            Source.Capture = true;
            Source.Cursor = Cursors.Cross;
            locked = true;
            Point Shouldbe = Point.Add(FormLocation, HalfFormSize);
            Cursor.Position = Shouldbe;
            Frame();
            Cursor.Hide();
            Locked?.Invoke(this, EventArgs.Empty);
        }

        public void Unlock()
        {
            if (!locked) return;
            locked = false;
            Source.Capture = false;
            Source.Cursor = Cursors.Default;
            Cursor.Clip = new Rectangle(0, 0, 0, 0);
            Cursor.Show();
            Unlocked?.Invoke(this, EventArgs.Empty);
        }

        int ButtonToInt(MouseButtons button)
        {
           switch(button)
            {
                case MouseButtons.Left:
                    return 0;
                case MouseButtons.Right:
                    return 1;
                case MouseButtons.Middle:
                    return 2;
                case MouseButtons.XButton1:
                    return 3;
                case MouseButtons.XButton2:
                    return 4;
            }
            return 0;
        }

        MouseButtons IntoToButton(int key)
        {
            switch (key)
            {
                case 0:
                    return MouseButtons.Left;
                case 1:
                    return MouseButtons.Right;
                case 2:
                    return MouseButtons.Middle;
                case 3:
                    return MouseButtons.XButton1;
                case 4:
                    return MouseButtons.XButton2;
            }
            return MouseButtons.Left;
        }

        public Mouse(Form Source)
        {
            this.Source = Source;
            Source.LostFocus += Source_LostFocus;
            Source.LocationChanged += (sender,e) => FormLocation = Source.DesktopLocation;
            Source.SizeChanged += (sender, e) => HalfFormSize = new Size(Source.Size.Width/2,Source.Size.Height/2);
            Source.Shown += (sender, e) =>
            {
                FormLocation = Source.DesktopLocation;
                HalfFormSize = new Size(Source.Size.Width / 2, Source.Size.Height / 2);
                Source.Activate();
            };

            Source.MouseDown += (sender, e) =>
            {
                base.KeyDown(ButtonToInt(e.Button));
            };

            Source.MouseUp += (sender, e) =>
            {
                base.KeyUp(ButtonToInt(e.Button));
            };

            Source.LostFocus += (sender, e) =>
            {
                Unlock();
            };
        }

        public bool IsDown(MouseButtons k)
        {
            return base.IsDown(ButtonToInt(k));
        }

        public bool IsUp(MouseButtons k)
        {
            return !base.IsDown(ButtonToInt(k));
        }

        public bool StateChanged(MouseButtons k)
        {
            return base.StateChanged(ButtonToInt(k));
        }

        /// <summary>
        /// Was down is now up, use frame() in loop
        /// </summary>
        public bool ChangedUp(MouseButtons k)
        {
            return base.ChangedUp(ButtonToInt(k));
        }
        /// <summary>
        /// Was up is now down, use frame() in loop
        /// </summary>
        public bool ChangedDown(MouseButtons k)
        {
            return base.ChangedDown(ButtonToInt(k));
        }

        private void Source_LostFocus(object sender, EventArgs e)
        {
            Unlock();
        }

        public float dX, dY;
        public event EventHandler<(float dX, float dY)> MouseDelta;

        Point FormLocation;
        Size HalfFormSize;

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public new void Frame()
        {
            if (!locked)
            {
                dX = 0;dY = 0;
                return;
            }

            Point Shouldbe = Point.Add(FormLocation, HalfFormSize);
            Point IsReally = Cursor.Position;

            dX = IsReally.X - Shouldbe.X;
            dY = IsReally.Y - Shouldbe.Y;
            MouseDelta?.Invoke(this, (dX, dY));

            Cursor.Position = Shouldbe;
            base.Frame();
        }

        /// <summary>
        /// Gets Pixel in area of Screen. Can be used for raycasting from mouse
        /// </summary>
        /// <param name="WiewPortScreen">Start of 2d Area on screen (TopLeft Pixel Location)</param>
        /// <param name="width">Width of 2d area</param>
        /// <param name="height">Height of 2d area</param>
        public Vector2 GetPixelXY(Vector2 WiewPortScreen, Vector2 WiewPortSize, Vector2 CanvasResolution)
        {
            Vector2 Cursorpos = new Vector2(Cursor.Position.X, Cursor.Position.Y);
            Vector2 delta = (Cursorpos - WiewPortScreen) * CanvasResolution / WiewPortSize;

            return Vector2.Min(Vector2.Max(Vector2.Zero, delta), CanvasResolution); //Clamp to width, height
        }
    }
}
