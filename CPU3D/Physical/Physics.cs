using CPU3D.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using CPU3D.RayTrace;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Physical
{
    public class Physics
    {
        public Vector3 Speed = Vector3.Zero;
        public Vector3 Force = -Vector3.UnitY * 10;

        public Object3D obj { get; }

        public void UpdateForces(float dT)
        {
            Speed += Force * dT;
        }

        public void Animate(float dT)
        {
            
        }

        public Physics(Object3D object3D)
        {
            obj = object3D;
        }

    }
}
