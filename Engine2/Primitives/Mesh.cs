using Engine2.Raytrace;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Primitives
{
    public struct Face
    {
        public int A;
        public int B;
        public int C;
    }
    public class GameObject
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }

        Vector3 rot = Vector3.Zero;
        public Vector3 Rotation
        {
            get => rot; set
            {
                float cap = (float)(2 * Math.PI);
                rot = new Vector3(value.X % cap, value.Y % cap, value.Z % cap);
            }
        }

        public RecursiveBox Bounds { get; private set; }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.RotationYawPitchRoll(this.Rotation.Y, this.Rotation.X, this.Rotation.Z) * Matrix.Translation(this.Position);
            }
        }

        public GameObject(string name, int verticesCount, int facesCount)
        {
            Vertices = new Vertex[verticesCount];
            Faces = new Face[facesCount];
            Name = name;
        }

        public void CreateBoundingbox()
        {
            Bounds = new RecursiveBox(this);
            Bounds.Calculate();
        }

    }

    public struct Vertex
    {
        public Vector3 Normal;
        public Vector3 Coordinates;
        public Vector3 WorldCoords;
    }
}
