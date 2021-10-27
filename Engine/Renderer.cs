using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Engine.Render;
using System.Threading;
using System.Collections.Concurrent;
using Engine.Drawable;
using System.Numerics;

namespace Engine
{
    class Renderer
    {
        public DirectBitmap buffer;
        Thread WorkConsume;
        BlockingCollection<Action> Renderquere = new BlockingCollection<Action>();
        AutoResetEvent RenderComplete = new AutoResetEvent(false);
        public NativeColor BackColor = Color.Green;

        public List<Object2D> GameObjects = new List<Object2D>();

        public int Width => buffer.Width;
        public int Height => buffer.Height;
        public Image Image => buffer.Bitmap;

        public Renderer(int Width, int Height)
        {
            buffer = new DirectBitmap(Width, Height);
            WorkConsume = new Thread(new ThreadStart(TakeWork)) { IsBackground = true };
            WorkConsume.Start();
        }

        void TakeWork()
        {
            while(true)
            {
                Action f = Renderquere.Take();
                f();
            }
        }

        public void Resize(int width, int height)
        {
            if (buffer != null) buffer.Dispose();
            buffer = new DirectBitmap(width, height);
        }

        void Enqueue(Action drawable)
        {
            Renderquere.Add(drawable);
        }

        void RenderObjects2D()
        {
            Image2D[] Images = GameObjects.OfType<Image2D>().ToArray();

            Image2D img0 = Images[0];

            buffer.InvokeOnPixel((f) =>
            {
                var (x, y, i) = f;

                buffer.Pixel[i] = BackColor;

                img0.Render(buffer, x, y);
            });
        }



        public void Draw2D()
        {
            Enqueue(() => RenderObjects2D());
        }

        public void Clear()
        {
            BackColor = 0;
        }
        public void Clear(Color Color)
        {
            BackColor = Color.ToArgb();
        }

        public void WaitRenderComplete()
        {
            Enqueue(() => RenderComplete.Set());
            RenderComplete.WaitOne();
        }
    }
}
