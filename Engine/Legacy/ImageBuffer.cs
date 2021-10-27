using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Legacy
{
    public class ImageBuffer
    {
        static byte[] BM_ASCII = Encoding.ASCII.GetBytes("BM");
        static int BM_ZERO = 0;
        static int BM_OFFSET = 54;
        static int BM_INFOSIZE = 40;
        static Int16 BM_PLANES = 1;
        static Int16 BM_COLORBITS = 32;

        static Int16 BM_INT16NULL = 0;
        static int BM_COMPRESSION = 0;

        byte[] Data;
        public MemoryStream DataStream;
        public int Width;
        public int Height;

        public int this[int X, int Y]
        {
            get
            {
                if (X < 0 || X >= this.Width || Y < 0 || Y >= this.Height) return -1;
                return this[Width * Y + X];
            }
            set
            {
                if (X < 0 || X >= this.Width || Y < 0 || Y >= this.Height) return;
                this[Width * Y + X] = value;
            }
        }

        /// <summary>
        /// Gets Pixel At Index I, refer to this[X,Y] for X,Y acces
        /// </summary>
        public int this[int i]
        {
            get
            {
                return BitConverter.ToInt32(Data, BM_OFFSET + i * 4);
            }
            set
            {
                Data[BM_OFFSET + i * 4 + 0] = (byte)(value >> 0);
                Data[BM_OFFSET + i * 4 + 1] = (byte)(value >> 8);
                Data[BM_OFFSET + i * 4 + 2] = (byte)(value >> 0x10);
                Data[BM_OFFSET + i * 4 + 3] = (byte)(value >> 0x18);
            }
        }

        public ImageBuffer(int Width, int Height)
        {
            CreateBuffer(Width, Height);
        }

        void CreateBuffer(int Width, int Height)
        {
            int length = Width * Height * 4; //32 bit image
            int BM_LENGTH = BM_OFFSET + length; // total bmp size

            Data = new byte[BM_LENGTH];
            DataStream = new MemoryStream(Data);
            this.Width = Width;
            this.Height = Height;
            using (BinaryWriter writer = new BinaryWriter(DataStream, Encoding.ASCII, true))
            {
                writer.Write(BM_ASCII);
                writer.Write(BM_LENGTH);
                writer.Write(BM_INT16NULL);
                writer.Write(BM_INT16NULL);
                writer.Write(BM_OFFSET);
                writer.Write(BM_INFOSIZE);

                writer.Write(Width);
                writer.Write(Height);
                writer.Write(BM_PLANES);
                writer.Write(BM_COLORBITS);
                writer.Write(BM_COMPRESSION);
                writer.Write(length); //length
                writer.Write(BM_ZERO);
                writer.Write(BM_ZERO);
                writer.Write(BM_ZERO);
                writer.Write(BM_ZERO);
            }

            DataStream.Position = 0;
        }

        public void Resize(int width, int height)
        {
            CreateBuffer(width, height);
        }
    }
}
