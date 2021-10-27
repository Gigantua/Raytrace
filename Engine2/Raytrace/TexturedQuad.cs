using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Raytrace
{
    class TexturedQuad
    {
        Vector3 A, B, C, D;
        public TexturedQuad(Vector3 A,Vector3 B,Vector3 C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = (A - B) + C;
        }



    }
}
