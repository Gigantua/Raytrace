using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CPU3D.RayTrace;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public abstract class CanvasBase<TColor,TPixel,TSubPixel> : IDisposable where TColor : IColor<TSubPixel,TPixel>
    {
        GCHandle handle;
        public TColor[] Pixel { get; set; }
        public int BytesPerPixel => Marshal.SizeOf<TColor>();
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ScanlineBytes { get; private set; }
        public long BytesCount { get; private set; }
        public long PixelCount { get; private set; }
        public Paint<TColor, TPixel, TSubPixel> Draw { get; private set; }
        public IntPtr Pointer => handle.AddrOfPinnedObject();

        readonly Vector2 Lookup;

        /// <summary>
        /// 4 byte float per component A,R,G,B = 128 bit per pixel
        /// </summary>
        public CanvasBase(int width, int height)
        {
            this.PixelCount = Math.BigMul(width, height);
            Pixel = new TColor[this.PixelCount];
            handle = GCHandle.Alloc(Pixel, GCHandleType.Pinned);
            this.Width = width;
            this.Height = height;
            this.BytesCount = Math.BigMul(width * height, Marshal.SizeOf<TPixel>()); //Can be bigger than int32 for 16k+
            this.ScanlineBytes = Width * Marshal.SizeOf<TPixel>();
            Draw = new Paint<TColor,TPixel,TSubPixel>(this);
            this.Lookup = new Vector2(1, Width);
        }

        public void Dispose()
        {
            handle.Free();
            Pixel = null;
        }

        public TColor this[Vector2 xy]
        {
            get => Pixel[(int)Vector2.Dot(xy, Lookup)]; //the same as Width * Y + X
            set => Pixel[(int)Vector2.Dot(xy, Lookup)] = value;
        }

        public TColor this[int X, int Y]
        {
            get => Pixel[Width * Y + X];
            set => Pixel[Width * Y + X] = value;
        }

        public TColor this[int i]
        {
            get => Pixel[i];
            set => Pixel[i] = value;
        }

        public abstract CanvasBase<TColor, TPixel, TSubPixel> Copy();

        public void CopyTo(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(ToByteArray());
        }

        public void CopyTo(CanvasBase<TColor, TPixel, TSubPixel> other)
        {
            Array.Copy(this.Pixel, other.Pixel, Math.Min(this.Pixel.Length, other.Pixel.Length));
        }

        public byte[] ToByteArray()
        {
            byte[] block = new byte[BytesCount];
            var hndle = GCHandle.Alloc(block);
            Marshal.Copy(Pointer, block, 0, (int)BytesCount);
            hndle.Free();
            return block;
        }


    }
}
