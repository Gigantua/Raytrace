//#define SLOWCAST
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CPU3D.RayTrace;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public class Paint<TColor, TPixel, TSubPixel> where TColor : IColor<TSubPixel, TPixel>
    {
        CanvasBase<TColor,TPixel,TSubPixel> Canvas;
        public Paint(CanvasBase<TColor, TPixel, TSubPixel> Canvas)
        {
            this.Canvas = Canvas;
        }


        public void Clear(TColor color)
        {
            parallel((x) =>
            {
                return color;
            });
        }

        void parallel(Func<TColor, TColor> Execution)
        {
            Parallel.For(0, Canvas.PixelCount, (i) =>
            {
                Canvas.Pixel[i] = Execution(Canvas.Pixel[i]);
            });
        }

        public void PixelInvoke(Func<Vector2, TColor> shader)
        {
            Parallel.For(0, Canvas.Height, (y) =>
            {
                for(int x = 0; x < Canvas.Width; x++)
                {
                    Canvas[x, y] = shader(new Vector2(x, y));
                }
            });
        }

        IEnumerable<int> SteppedIterator(int startIndex, int endEndex, int stepSize)
        {
            for (int i = startIndex; i < endEndex; i += stepSize)
            {
                yield return i;
            }
        }

        public static int threadcount = Environment.ProcessorCount;
        static ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = threadcount };

        static Paint()
        {
            threadcount = Environment.ProcessorCount;
        }

        public void CheckerboardInvoke(ref bool toggle,Func<Vector2, TColor> shader)
        {
            int checkertoggle = toggle ? 0 : 1;

            Parallel.For(0, Canvas.Height, options, (y)=>
            {
                for (int x = 0; x < Canvas.Width; x++)
                {
                    if ((x + y) % 2 == checkertoggle)
                    {
                        Canvas[x, y] = shader(new Vector2(x , y));
                    }
                }
            });
            
            toggle = !toggle;
        }

        static readonly Vector2 NotFive = new Vector2(0.5f, 0.5f);

        public void GetUV(int ScreenWidth, int ScreenHeight, Vector2 zoom, Func<Vector2, TColor> shader)
        {
            int canvaswidth = Canvas.Width;
            int canvasheight = Canvas.Height;

            Vector2 widthheight = new Vector2(ScreenWidth, ScreenHeight);
            float ratio = widthheight.X / widthheight.Y;

            TColor[] Pixel = Canvas.Pixel;
            int PixelWidth = Canvas.Width;


#if DEBUG
            for(int y = 0; y < canvasheight; y++)
            {
                for (int x = 0; x < canvaswidth; x++)
                {
                    Vector2 ScreenXY = new Vector2(x, y);
                    Vector2 UV = (ScreenXY / widthheight - NotFive) * zoom;

                    Pixel[y * PixelWidth + x] = shader(UV);
                }
            };
#else
            Parallel.ForEach(SteppedIterator(0, canvasheight, 1), new ParallelOptions() { MaxDegreeOfParallelism = threadcount }, (y) =>
            {
                for (int x = 0; x < canvaswidth; x++)
                {
                    Vector2 ScreenXY = new Vector2(x, y);
                    Vector2 UV = (ScreenXY / widthheight - NotFive) * zoom;

                    Pixel[y * PixelWidth + x] = shader(UV);
                }
            });
#endif

        }

        public delegate bool Rayfunction(Vector2 xy, out TColor c); //Screen XY to Color

        public void GetUVCheckerBoard(ref bool toggle,Vector2 widthheight, float ratio, Vector2 zoom, Rayfunction shader)
        {
            int checkertoggle = toggle ? 0 : 1;
            int canvaswidth = Canvas.Width;
            int canvasheight = Canvas.Height;

            TColor[] Pixel = Canvas.Pixel;
            int PixelWidth = Canvas.Width;
            
            Parallel.For(0, canvasheight, options, (y) =>
            {
                for (int x=0; x < canvaswidth; x++)
                {
                    if ((x + y) % 2 != checkertoggle) continue;

                    Vector2 ScreenXY = new Vector2(x, y);
                    Vector2 UV = (ScreenXY / widthheight - NotFive) * zoom;
                    bool ok = shader(UV, out TColor c);
                    if (ok) Pixel[y * PixelWidth + x] = c; //This happens 99.9%
                    else //We chose a neighbour to color in because ray could not be calculated (we hit a thin edge)
                    {
                        if (y == 0) continue;
                        if (x == canvaswidth - 1) continue;
                        if (y == 0) continue;
                        if (y == canvasheight - 1) continue;

                        if (Pixel is Color8[] pixels) //Pick first non black pixel
                        {
                            Color8 l = pixels[y * PixelWidth + (x - 1)];
                            if (l != Color8.Black) { pixels[y * PixelWidth + x] = l; continue; }

                            Color8 r = pixels[y * PixelWidth + (x + 1)];
                            if (r != Color8.Black) { pixels[y * PixelWidth + x] = r; continue; }

                            Color8 u = pixels[(y + 1) * PixelWidth + x];
                            if (u != Color8.Black) { pixels[y * PixelWidth + x] = u; continue; }

                            Color8 d = pixels[(y - 1) * PixelWidth + x];
                            if (d != Color8.Black) { pixels[y * PixelWidth + x] = d; continue; }

                            pixels[y * PixelWidth + x] = Color8.Black;
                        }
                        else
                        {
                            Pixel[y * PixelWidth + x] = Pixel[(y - 1) * PixelWidth + x];
                        }
                    }
                }
            });

            toggle = !toggle;
        }

        /// <summary>
        /// Passes up,left,right,bottom pixel. Is not invoked for edges
        /// </summary>
        public void Neighbourinvoke(Func<TColor, TColor, TColor, TColor, TColor, TColor> shader)
        {
            var copy = Canvas.Copy();

            Parallel.For(1, Canvas.Height-1, (y) =>
            {
                for (int x = 1; x < Canvas.Width-1; x++)
                {
                    copy[x, y] = shader(Canvas[x, y - 1], Canvas[x - 1, y], Canvas[x + 1, y], Canvas[x, y + 1], Canvas[x,y]);
                }
            });

            copy.CopyTo(Canvas);
        }

    }

}
