using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Brush
    {
        public NativeColor Color;
        public bool DoAntialiasing = true;

        public Brush(Color color)
        {
            this.Color = color.ToArgb();
        }
    }
}
