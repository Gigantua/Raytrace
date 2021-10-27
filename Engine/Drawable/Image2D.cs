using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Render;
using System.Drawing;
using System.Drawing.Imaging;

namespace Engine.Drawable
{
    class Image2D : Object2D
    {
        public DirectBitmap map;

        double alpha = 1;
        public double Alpha
        {
            get => alpha;
            set
            {
                if (value != alpha)
                {
                    alpha = value;
                    UpdateAlpha();
                }
            }
        }


        void UpdateAlpha()
        {
            Parallel.For(0,map.Pixel.Length,(i)=>
            {
                NativeColor pixel = map.Pixel[i];
                map.Pixel[i] = new NativeColor(pixel.R, pixel.G, pixel.B, (byte)(alpha * byte.MaxValue));
            });
        }


        public Image2D(Bitmap source)
        {
            map = new DirectBitmap(source);
            this.WidthHeight = new Vector2D(source.Width, source.Height);
        }

        public static Image2D FromFile(string path)
        {
            Bitmap orig = new Bitmap(path);
            Bitmap clone = new Bitmap(orig.Width, orig.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(orig, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            orig.Dispose();
            return new Image2D(clone);
        }
        

        protected override void _Render(DirectBitmap Target, int ScreenX, int ScreenY, double LocalX, double LocalY)
        {
            Target[ScreenX, ScreenY] += this.map[(int)LocalX, (int)LocalY];
        }
    }
}
