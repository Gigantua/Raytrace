using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    public interface IColor<TSubPixel,TPixel>
    {
        TSubPixel R { get; }
        TSubPixel G { get; }
        TSubPixel B { get; }
        TSubPixel A { get; }
        TPixel RGB { get; }

        int ToInt();
    }

}
