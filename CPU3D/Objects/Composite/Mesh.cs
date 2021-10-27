using CPU3D.RayTrace;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Objects
{
    class Mesh : Object3D
    {
        Vector3[] vertices; //All Points
        Vector3[] faces; //All Triangles x,y,z is integer

        public Triangle[] Faces;

        public Mesh(Vector3[] Vertices, Vector3[] Faces)
        {
            this.vertices = Vertices;
            this.faces = Faces;

            this.Faces = new Triangle[faces.Length];
            for(int i=0;i< faces.Length; i++)
            {
                this.Faces[i] = new Triangle(vertices, faces[i]);
            }
        }

        public override HitInfo Hittest(Ray r) => null;
    }
}
