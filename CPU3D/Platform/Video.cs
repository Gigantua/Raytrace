using Accord.Video.FFMPEG;
using CPU3D.Draw;
using SharpDX;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPU3D.Platform
{
    public enum Codec
    {
        H264,
        Bitmaps
    }

    public class Video
    {
        VideoFileWriter fr = new Accord.Video.FFMPEG.VideoFileWriter();
        int width;int height;
        Codec codec;
        string path;

        string folder;
        string extension;
        string filename;

        public Video(string path,int width,int height, Codec codec = Codec.H264)
        {
            this.width = width;
            this.height = height;
            this.codec = codec;
            this.path = path;

            this.folder = Path.GetDirectoryName(path);
            this.extension = Path.GetExtension(path);
            this.filename = Path.GetFileNameWithoutExtension(path);

            switch(codec)
            {
                case Codec.H264:
                    fr.BitRate = (int)150E7;
                    fr.Width = width;
                    fr.Height = height;
                    fr.FrameSize = 0;
                    fr.VideoCodec = VideoCodec.H264;
                    fr.Open(path);
                    break;
                case Codec.Bitmaps:
                    Task.Factory.StartNew(WriteThread, TaskCreationOptions.LongRunning);
                    Task.Factory.StartNew(WriteThread, TaskCreationOptions.LongRunning);
                    Task.Factory.StartNew(WriteThread, TaskCreationOptions.LongRunning);
                    Task.Factory.StartNew(WriteThread, TaskCreationOptions.LongRunning);
                    this.extension = ".Jpeg";
                    break;
            }
        }

        void WriteThread()
        {
            while(!workstack.IsCompleted)
            {
                var f = workstack.Take();
                f();
            }
        }

        BlockingCollection<Action> workstack = new BlockingCollection<Action>(new ConcurrentQueue<Action>(), 12);
        Stopwatch dT;

        public (Bitmap,IntPtr) BitmapFromPointer(IntPtr Scan0)
        {
            long size = Math.BigMul(width, height) * 4; //Bigger than 2gb frame
            IntPtr formated = Marshal.AllocHGlobal(new IntPtr(size));
            var bmp = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, formated);

            unsafe
            {
                byte* Source = (byte*)Scan0;
                byte* Dest = (byte*)formated;
                for (long c = 0; c < size; c += 4)
                {
                    byte sr = Source[c + 0];
                    byte sg = Source[c + 1];
                    byte sb = Source[c + 2];
                    byte sa = Source[c + 3];

                    Dest[c + 0] = sb; //blue
                    Dest[c + 1] = sg; //green
                    Dest[c + 2] = sr; //red
                    Dest[c + 3] = sa;
                }
            }
            return (bmp,formated);
        }


        long Frames = 0;
        public void WriteFrame(IntPtr Scan0)
        {
            (Bitmap bmp, IntPtr ptr) = BitmapFromPointer(Scan0);
            try
            {
                if (dT == null)
                {
                    dT = Stopwatch.StartNew();
                }
                if (codec == Codec.Bitmaps)
                {
                    //PointerToDisk.Save(path, width, height, Scan0);

                    string imgpath = $"{folder}\\{filename}_{Frames}_T+{dT.Elapsed.TotalSeconds.ToString("0.00000")}s{extension}";
                    workstack.Add(() =>
                    {
                        bmp.Save(imgpath, ImageFormat.Bmp);
                        bmp.Dispose();
                        Marshal.FreeHGlobal(ptr);
                    });
                }
                else
                {
                    fr.WriteVideoFrame(bmp, dT.Elapsed);
                    bmp.Dispose();
                    Marshal.FreeHGlobal(ptr);
                }
            }
            catch { }
            Frames++;
        }

        bool Saved;
        ~Video()
        {
            if (!Saved) Save();
        }

        public void Save()
        {
            if (codec == Codec.Bitmaps)
            {
                workstack.CompleteAdding();
            }
            else
            {
                fr.Close();
            }
            Saved = true;
        }
    }


}
