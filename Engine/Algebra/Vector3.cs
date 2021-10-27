using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Algebra
{
    public class Vector3
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3()
        {

        }

        public Vector3(double X,double Y,double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vector3(Vector2D vec)
        {
            this.X = vec.dX;
            this.Y = vec.dY;
            this.Z = 1;
        }

    }
}
