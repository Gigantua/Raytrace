using System;
using System.Collections.Generic;
using System.Linq;
using CPU3D.RayTrace;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Objects
{
    public class Camera
    {
        const float Pi2 = (float)(2 * Math.PI);
        const float Pihalf = (float)(0.5 * Math.PI);

        public double HorizonMin = 3 * Math.PI / 180;
        public double HorizonMax = 177 * Math.PI / 180;

        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Right;
        public Vector3 Up;


        public float FOV
        {
            get;
            private set;
        }

        public float HalfTan_FOV;
        public Vector2 HalfTanFOV2;


        public float FOV_Degrees
        {
            get
            {
                return (int)(FOV * 180 / Math.PI);
            }
            set
            {
                FOV = (float)(value * Math.PI / 180);
                HalfTan_FOV = (float)Math.Tan(FOV / 2);
                HalfTanFOV2 = 2 * new Vector2(HalfTan_FOV, HalfTan_FOV);
            }
        }
        public Camera()
        {
            this.Position = Vector3.Zero;
            this.Direction = new Vector3(0, 0, 1);
            this.Right = new Vector3(1, 0, 0);
            this.Up = Vector3.Normalize(Vector3.Cross(Direction, Right));
            this.FOV_Degrees = 65;
        }

        public Camera(Vector3 pos, Vector3 direction, float FOV_Degrees)
        {
            this.Position = Vector3.Zero;
            this.Direction = Vector3.Normalize(direction);
            this.Right = Vector3.Normalize(Vector3.Cross(Direction, new Vector3(0,-1,0)));
            this.Up = Vector3.Normalize(Vector3.Cross(Direction, Right));
            this.FOV_Degrees = FOV_Degrees;
        }



        public override string ToString()
        {
            return $"POS:{Position} Direction:{Direction} Right:{Up}";
        }

        public void MoveForward(float delta)
        {
            Position += Direction * delta;
        }

        public void Strave(float delta)
        {
            Position += Right * delta;
        }

        public void Rotate(float delta)
        {
            var leftright = Matrix3x3.CreateFromAxisAngle(Direction, delta);
            Right = Matrix3x3.Transform(Right, leftright);
            this.Up = Vector3.Normalize(Vector3.Cross(Direction, Right));
        }

        /// <summary>
        /// Positive X is right. Positive Y is up. (in radiant)
        /// </summary>
        public void RotateScreen(float dX_Rad, float dY_Rad)
        {
            var leftright = Matrix3x3.CreateFromAxisAngle(Vector3.UnitY, dX_Rad);
            //var leftright = Matrix4x4.CreateFromAxisAngle(Up, dX_Rad);
            Direction = Matrix3x3.Transform(Direction, leftright);
            Right = Matrix3x3.Transform(Right, leftright);
            this.Up = Vector3.Normalize(Vector3.Cross(Direction, Right));

            double angle = Math.Acos(Vector3.Dot(Vector3.UnitY, Direction));

            var updown = Matrix3x3.CreateFromAxisAngle(Right, dY_Rad);
            Vector3 NewDirection = Matrix3x3.Transform(Direction, updown);

            double delta = Math.Acos(Vector3.Dot(NewDirection, Direction));

            if (angle - delta < HorizonMin && dY_Rad < 0) //block more downward rotation
            {
                return;
            }

            if (angle + delta > HorizonMax && dY_Rad > 0)//block more upward rotation
            {
                return;
            }

            Direction = NewDirection;
            this.Up = Vector3.Normalize(Vector3.Cross(Direction, Right));
        }



    }
}
