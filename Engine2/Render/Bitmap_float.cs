using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Render
{
    class Bitmap_float : IDisposable
    {
        GCHandle handle;
        public Color4[] Pixel;
        public int Width;
        public int Height;
        public long BytesCount;

        public IntPtr Pointer => handle.AddrOfPinnedObject();

        /// <summary>
        /// 4 byte float per component A,R,G,B = 32 bytes per pixel
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Bitmap_float(int width,int height)
        {
            Pixel = new Color4[width * height];
            handle = GCHandle.Alloc(Pixel, GCHandleType.Pinned);
            Width = width;
            Height = height;
            BytesCount = Math.BigMul(width * height, Marshal.SizeOf<Color4>()); //Can be bigger than int32 for 16k+
        }


        public void Dispose()
        {
            handle.Free();
            Pixel = null;
        }


        public static Bitmap_float FromFile(string path)
        {
            Bitmap orig = new Bitmap(path);
            Bitmap bmp = new Bitmap(orig.Width, orig.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(orig, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            orig.Dispose();

            int[] pixelInts = new int[bmp.Width * bmp.Height];
            BitmapData pixelData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelData.Scan0, pixelInts, 0, pixelInts.Length);
            bmp.UnlockBits(pixelData);

            Bitmap_float flt = new Bitmap_float(bmp.Width, bmp.Height);
            Parallel.For(0, pixelInts.Length, (i) =>
            {
                flt[i] = Colorconvert.FromColor(Color.FromArgb(pixelInts[i]));
            });
            return flt;
        }

        public Bitmap_float CutImage(int X, int Y, int width, int height)
        {
            Bitmap_float Result = new Bitmap_float(width, height);

            Parallel.For(Y, Y+height, (y) =>
            {
                for (int x = X; x < X + width; x++)
                {
                    int targetx = x - X;
                    int targety = y - Y;

                    Result[targetx, targety] = this[x, y];
                }
            });

            

            return Result;
        }

        public static Bitmap_float FromImage(Image original, bool dispose = false)
        {
            Bitmap bmp = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            if (dispose) original.Dispose();

            int[] pixelInts = new int[bmp.Width * bmp.Height];
            BitmapData pixelData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelData.Scan0, pixelInts, 0, pixelInts.Length);
            bmp.UnlockBits(pixelData);

            Bitmap_float flt = new Bitmap_float(bmp.Width, bmp.Height);
            Parallel.For(0, pixelInts.Length, (i) =>
            {
                flt[i] = Colorconvert.FromColor(Color.FromArgb(pixelInts[i]));
            });
            return flt;
        }

        public Color4 this[int X, int Y]
        {
            get
            {
                return Pixel[Width * Y + X];
            }
            set
            {
                Pixel[Width * Y + X] = value;
            }
        }

        public Color4 this[int i]
        {
            get
            {
                return Pixel[i];
            }
            set
            {
                Pixel[i] = value;
            }
        }

        public int Clamp(int value, int max)
        {
            int min = 0;
            return (value < min) ? min : (value > max) ? max : value;
        }

        public Color4 GetPixel(float x, float y)
        {
            return this[Clamp((int)(x * Width), Width - 1), Clamp((int)(y * Height), Height - 1)];
        }

        public Color4 GetPixel(int x,int y)
        {
            return this[Clamp(x, Width-1), Clamp(y, Height-1)];
        }

        public Color4 AverageAround(int x, int y)
        {
            Color4 tl = GetPixel(x - 1, y - 1);
            Color4 t = GetPixel(x , y - 1);
            Color4 tr = GetPixel(x + 1, y - 1);

            Color4 ml = GetPixel(x - 1, y);
            Color4 mr = GetPixel(x + 1, y);

            Color4 bl = GetPixel(x - 1, y + 1);
            Color4 b = GetPixel(x, y + 1);
            Color4 br = GetPixel(x + 1, y + 1);

            return tl + t + tr + ml + mr + bl + b + br;
        }
    }
}
