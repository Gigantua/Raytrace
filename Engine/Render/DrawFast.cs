using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Render
{
    public static class DrawFast
    {

        public static void DrawImage(Bitmap source, Graphics target,int width,int heigth)
        {
            //target.DrawImage(source, 0, 0);

            target.InterpolationMode = InterpolationMode.HighQualityBicubic;
            target.SmoothingMode = SmoothingMode.HighQuality;
            target.PixelOffsetMode = PixelOffsetMode.HighQuality;
            target.CompositingQuality = CompositingQuality.HighQuality;

            // Figure out the ratio
            double ratioX = (double)width / (double)source.Width;
            double ratioY = (double)heigth / (double)source.Height;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(source.Height * ratio);
            int newWidth = Convert.ToInt32(source.Width * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((width - (source.Width * ratio)) / 2);
            int posY = Convert.ToInt32((heigth - (source.Height * ratio)) / 2);

            target.Clear(Color.White); // white padding
            target.DrawImage(source, posX, posY, newWidth, newHeight);
        }
    }
}
