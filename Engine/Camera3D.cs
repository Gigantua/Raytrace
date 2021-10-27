using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Camera3D
    {
        public Point3D Position;
        public Vector3D Orientation;
        public double ScreenWidth, ScreenHeight;

        public Camera3D(double X,double Y,double Z,double RotX,double RotY,double RotZ)
        {
            this.Position = new Point3D(X, Y, Z);
            this.Orientation = new Vector3D(RotX, RotY, RotZ);
        }
    }
}
