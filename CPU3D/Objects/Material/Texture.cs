using CPU3D.Draw;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU3D.RayTrace
{
    public class Texture
    {
        public Canvas Pixel;
        readonly float W;
        readonly float H;
        readonly Vector2 WH;

        public Texture(string Path)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(Path);
            Pixel = new Canvas(bmp.Width, bmp.Height);

            //new Form() { BackgroundImage = bmp, BackgroundImageLayout = ImageLayout.Zoom }.ShowDialog();

            W = bmp.Width - 2;
            H = bmp.Height - 2;
            WH = new Vector2(W, H);
            //new Form() { BackgroundImage = clone, BackgroundImageLayout = ImageLayout.Zoom }.ShowDialog();

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int width = bmp.Width;
            int height = bmp.Height;

            int bytesPerPixel = 3; // we assume that image is Format32bppArgb
            int maxPointerLenght = width * height * bytesPerPixel;
            int stride = width * bytesPerPixel;

            unsafe
            {
                byte* scan0 = (byte*)data.Scan0.ToPointer();

                int r = 0;
                for (int i = 0; i < maxPointerLenght; i += bytesPerPixel)
                {
                    byte B = scan0[i + 0];
                    byte G = scan0[i + 1];
                    byte R = scan0[i + 2];

                    Color8 sample = (R, G, B);
                    Pixel[r] = Colors.Convertfloat(sample);
                    r ++;
                }
            }
           

            bmp.UnlockBits(data);
            bmp.Dispose();
        }


        public CPU3D.Draw.Color ColorFromUV(Vector2 UV)
        {
            return Pixel[(int)(UV.X * W), (int)(UV.Y * H)];
        }

        static readonly Vector2 notfive = new Vector2(0.5f, 0.5f);

        static readonly Vector2 E1 = new Vector2(1, 0);
        static readonly Vector2 E2 = new Vector2(0, 1);
        static readonly Vector2 E3 = new Vector2(1, 1);
        public CPU3D.Draw.Color BilinearColorFromUV(Vector2 UV)
        {
            Vector2 uv = UV * WH;
            int x = (int)uv.X;
            int y = (int)uv.Y;
            Vector2 xy = new Vector2(x, y);
            Vector2 uvratio = uv - xy;
            Vector2 uvop = Vector2.One - uvratio;

            CPU3D.Draw.Color A = Pixel[x, y];
            CPU3D.Draw.Color B = Pixel[x + 1, y];
            CPU3D.Draw.Color C = Pixel[x, y + 1];
            CPU3D.Draw.Color D = Pixel[x + 1, y + 1];
            
            CPU3D.Draw.Color result = (A * uvop.X + B * uvratio.X) * uvop.Y + 
                                      (C * uvop.X + D * uvratio.X) * uvratio.Y;

            return result;
        }

    }
}
