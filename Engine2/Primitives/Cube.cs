using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Primitives
{
    class Cube : GameObject
    {
        public Cube() : base("Cube",8,12)
        {
            this.Faces[0] = new Face { A = 0, B = 1, C = 2 };
            this.Faces[1] = new Face { A = 1, B = 2, C = 3 };
            this.Faces[2] = new Face { A = 1, B = 3, C = 6 };
            this.Faces[3] = new Face { A = 1, B = 5, C = 6 };
            this.Faces[4] = new Face { A = 0, B = 1, C = 4 };
            this.Faces[5] = new Face { A = 1, B = 4, C = 5 };

            this.Faces[6] = new Face { A = 2, B = 3, C = 7 };
            this.Faces[7] = new Face { A = 3, B = 6, C = 7 };
            this.Faces[8] = new Face { A = 0, B = 2, C = 7 };
            this.Faces[9] = new Face { A = 0, B = 4, C = 7 };
            this.Faces[10] = new Face { A = 4, B = 5, C = 6 };
            this.Faces[11] = new Face { A = 4, B = 6, C = 7 };
        }

    }
}
