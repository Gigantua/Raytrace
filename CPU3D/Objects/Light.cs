using CPU3D.Draw;
using System;
using System.Collections.Generic;
using System.Linq;
using CPU3D.RayTrace;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Objects
{
    public class Light
    {
        public Vector3 Position;
        public Color Lightcolor; 

        public Light(Vector3 Pos)
        {
            this.Position = Pos;
        }
        public Light(float X,float Y,float Z)
        {
            this.Position = new Vector3(X, Y, Z);
        }
    }
}
