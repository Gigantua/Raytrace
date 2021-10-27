using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Engine2.Primitives
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; } = Vector3.Normalize(new Vector3(0.2f, -0.02f, 0));
        public Vector3 Target => Position + Direction;
        public Vector3 Up { get; private set; } = Vector3.UnitY;
        Vector3 Right => Vector3.Normalize(Vector3.Cross(Direction, Up));

        public override string ToString()
        {
            return Direction.ToString() + Position.ToString();
        }

        public Camera(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        public void MoveForward(float delta)
        {
            Vector3 dir = (Target - Position);
            Position += dir * (delta / dir.Length());
        }

        /// <summary>
        /// Positive X is right. Positive Y is up. (in radiant)
        /// </summary>
        public void Look(float dX,float dY)
        {
            var rotateright = Matrix3x3.RotationAxis(Up, dX);
            var rotateup = Matrix3x3.RotationAxis(Right, dY);
            Direction = Vector3.Transform(Direction, rotateright);
            Direction = Vector3.Transform(Direction, rotateup);
        }

        public void MoveRight(float delta)
        {
            Position += Right * delta;
        }

    }

}
