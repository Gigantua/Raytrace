using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RayForm
{
    public class FrameBuffer
    {
        readonly IntPtr _ptr;
        public IntPtr Pointer;
        public readonly int BytesPerPixel = 4;

        public FrameBuffer(int Width, int Height)
        {
            _ptr = Marshal.AllocHGlobal(Width * Height * 4 + 32);
            Pointer = IntPtr.Add(_ptr, (int)(_ptr.ToInt64() & 31)); //Align to 32 byte boundary
            
            this.Width = Width;
            this.Height = Height;
            Length = Width * Height;
        }


        ~FrameBuffer() => Marshal.FreeHGlobal(_ptr);

        public readonly int Width;
        public readonly int Height;
        public readonly int Length;
    }


    public class FrameBuffer8
    {
        public Memory<float> Pixel;
        public GCHandle FrameHandle;
        public IntPtr Pointer;

        public unsafe FrameBuffer8(int Width, int Height)
        {
            float[] _pixel = new float[Width * Height * 4 + 8];
            Pixel = _pixel; //Align to 8 * 4byte = 32byte boundary
            FrameHandle = GCHandle.Alloc(_pixel, GCHandleType.Pinned);

            long ptr = FrameHandle.AddrOfPinnedObject().ToInt64();
            long alignedaddress = (ptr + 32 - 1) & -32;
            Pointer = new IntPtr(alignedaddress);
            int offset = (int)(alignedaddress - ptr);
            Pixel = Pixel.Slice(offset);
            this.Width = Width;
            this.Height = Height;
            Length = Width * Height;
        }


        ~FrameBuffer8()
        {
            FrameHandle.Free();
            Pixel = null;
        }

        public readonly int Width;
        public readonly int Height;
        public readonly int Length;
    }
}
