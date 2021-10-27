using CPU3D.Draw;
using CPU3D.RayTrace;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.Physical;

namespace CPU3D.Objects
{
    
    public abstract class Object3D
    {
        public Material Material;
        public Physics physics;
        public AABB HitBox;

        public abstract HitInfo Hittest(Ray r);

        public Color GetColor(HitInfo hit) => Material.GetColor(hit);

        public Object3D()
        {
            physics = new Physics(this);
            Material = CPU3D.Objects.Material.Simple(Colors.White);
        }
    }
}
