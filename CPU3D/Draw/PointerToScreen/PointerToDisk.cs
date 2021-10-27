using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.IO;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public static class PointerToDisk
    {

        public static void Save(string path, int width, int height, IntPtr Location)
        {
            var factory = new ImagingFactory();

            WICStream stream = null;

            // ------------------------------------------------------
            // Encode a JPG image
            // ------------------------------------------------------

            // Create a WIC outputstream 
            if (File.Exists(path))
                File.Delete(path);

            stream = new WICStream(factory, path, NativeFileAccess.Write);

            // Initialize a Jpeg encoder with this stream
            var encoder = new PngBitmapEncoder(factory);
            encoder.Initialize(stream);

            // Create a Frame encoder
            var bitmapFrameEncode = new BitmapFrameEncode(encoder);
            bitmapFrameEncode.Options.ImageQuality = 0.8f;
            bitmapFrameEncode.Initialize();
            bitmapFrameEncode.SetSize(width, height);
            var guid = SharpDX.WIC.PixelFormat.Format24bppBGR;
            bitmapFrameEncode.SetPixelFormat(ref guid);

            // Write a pseudo-plasma to a buffer
            int stride = SharpDX.WIC.PixelFormat.GetStride(SharpDX.WIC.PixelFormat.Format24bppBGR, width);
            var bufferSize = height * stride;
            var buffer = new DataStream(bufferSize, true, true);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    buffer.WriteByte((byte)(x / 2.0 + 20.0 * Math.Sin(y / 40.0)));
                    buffer.WriteByte((byte)(y / 2.0 + 30.0 * Math.Sin(x / 80.0)));
                    buffer.WriteByte((byte)(x / 2.0));
                }
            }

            // Copy the pixels from the buffer to the Wic Bitmap Frame encoder
            bitmapFrameEncode.WritePixels(512, new DataRectangle(buffer.DataPointer, stride));

            // Commit changes
            bitmapFrameEncode.Commit();
            encoder.Commit();
            bitmapFrameEncode.Dispose();
            encoder.Dispose();
            stream.Dispose();

            // ------------------------------------------------------
            // Decode the previous JPG image
            // ------------------------------------------------------

            // Read input
            stream = new WICStream(factory, path, NativeFileAccess.Read);
            var decoder = new JpegBitmapDecoder(factory);
            decoder.Initialize(stream, DecodeOptions.CacheOnDemand);
            var bitmapFrameDecode = decoder.GetFrame(0);
            var queryReader = bitmapFrameDecode.MetadataQueryReader;

            // Dump MetadataQueryreader
            queryReader.Dump(Console.Out);
            queryReader.Dispose();

            bitmapFrameDecode.Dispose();
            decoder.Dispose();
            stream.Dispose();

            // Dispose
            factory.Dispose();
            System.Diagnostics.Process.Start(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path)));
        }

    }

}
