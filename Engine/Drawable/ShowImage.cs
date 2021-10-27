using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine.Drawable
{
    public static class ShowImage
    {
        public static void Show(Image source)
        {
            Form f = new Form();
            PictureBox box = new PictureBox();
            box.Dock = DockStyle.Fill;
            box.SizeMode = PictureBoxSizeMode.Zoom;
            box.Image = source;
            f.Controls.Add(box);
            f.ShowDialog();
        }

    }
}
