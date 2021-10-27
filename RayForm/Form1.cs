using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayForm
{
    public partial class MainForm : Form
    {
        public DirectDraw Render { get; private set; }

        Point? DragXY;

        public event EventHandler<(int dx, int dy)> MouseDragged;

        public MainForm(int Width, int Height)
        {
            InitializeComponent();
            this.Load += (sender, e) => Render = new DirectDraw(this.Handle, Width, Height, PixelFormat.RGBA8_Byte);
            this.MouseDown += (sender, e) => DragXY = e.Location;
            this.MouseMove += MainForm_MouseMove;
            this.MouseUp += (sender, e) => DragXY = null;
            this.LostFocus += (sender, e) => DragXY = null;
            this.Width = Width / 2;
            this.Height = Height / 2;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (DragXY == null) return;
            Point xy = DragXY.Value;

            int dx = e.Location.X - xy.X;
            int dy = e.Location.Y - xy.Y;

            DragXY = e.Location;
            MouseDragged?.Invoke(this, (dx, dy));
        }

    }
}
