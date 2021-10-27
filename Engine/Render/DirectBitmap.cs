using Engine.Drawable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Render
{
    public class DirectBitmap : IDisposable
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public Bitmap Bitmap { get; private set; }

        public Bitmap TopLeft { get; private set; }
        public Bitmap TopRight { get; private set; }
        public Bitmap BottomLeft { get; private set; }
        public Bitmap BottomRight { get; private set; }

        public NativeColor[] Pixel { get; private set; }

        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        public static DirectBitmap FromFile(string path)
        {
            MemoryStream mem = new MemoryStream();
            using (FileStream fs = File.OpenRead(path))
            {
                fs.CopyTo(mem);
            }
            return new DirectBitmap(Image.FromStream(mem));
        }

        protected GCHandle BitsHandle { get; private set; }

        public IntPtr Pointer => BitsHandle.AddrOfPinnedObject();
        public int PointerSize { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Pixel = new NativeColor[width * height];
            BitsHandle = GCHandle.Alloc(Pixel, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }

        public DirectBitmap(Image source) : this(source.Width,source.Height)
        {
            if (source.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException("Only Format32bppArgb allowed");
            }
            if (source is Bitmap == false)
            {
                throw new ArgumentException("Must be bitmap");
            }

            BitmapData data = (source as Bitmap).LockBits(new Rectangle(0, 0, this.Width, this.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int size = data.Height * data.Stride;

            CopyMemory(BitsHandle.AddrOfPinnedObject(), data.Scan0, (uint)(this.Width * this.Height * Marshal.SizeOf<NativeColor>()));
            PointerSize = this.Width * this.Height * Marshal.SizeOf<NativeColor>();

            (source as Bitmap).UnlockBits(data);
        }

        (int X, int Y) GetXY(int I)
        {
            return (I % this.Width, I / this.Width);
        }

        int GetI(int X, int Y)
        {
            return Width * Y + X;
        }

        public void InvokeOnPixel(Action<(int X,int Y,int Index)> DoXY)
        {
            int width = this.Width;
            int length = this.Pixel.Length;
            Parallel.For(0, length, (i) =>
            {
                int x;
                int y = Math.DivRem(i, width, out x);
                DoXY((x, y, i));
            });
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }

        public NativeColor this[int X, int Y]
        {
            get
            {
                if (X < 0 || X >= this.Width || Y < 0 || Y >= this.Height) return 0;
                return Pixel[Width * Y + X];
            }
            set
            {
                if (X < 0 || X >= this.Width || Y < 0 || Y >= this.Height) return;
                Pixel[Width * Y + X] = value;
            }
        }

        /// <summary>
        /// Gets Pixel At Index I, refer to this[X,Y] for X,Y acces
        /// </summary>
        public NativeColor this[int i] => Pixel[i];
    }
}
