using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayForm
{
    public class Camera1
    {
        public Camera1(Vector1 pos, Vector1 forward, Vector1 up, Vector1 right)
        {
            Pos = pos;
            Forward = forward;
            Up = up;
            Right = right;
        }

        public Vector1 Pos;
        public Vector1 Forward;
        public Vector1 Up;
        public Vector1 Right;

        public void MoveForward(float delta)
        {
            Pos += Forward * delta;
        }
        public void MoveRight(float delta)
        {
            Pos += Right * delta;
        }


        public static Camera1 Create(Vector1 pos, Vector1 lookAt)
        {
            Vector1 forward = (lookAt - pos).Normalize();
            Vector1 down = new Vector1(0, -1, 0);

            Vector1 right = Vector1.Cross(forward, down).Normalize();
            Vector1 up = Vector1.Cross(forward, right).Normalize();

            return new Camera1(pos, forward, up, right);
        }


        public void RotateLeft(double v)
        {
            float sin = (float)Math.Sin(v);
            float cos = (float)Math.Cos(v);
            var up = new Vector1(0, 1, 0);
            Forward = Forward * cos + Vector1.Cross(up, Forward) * sin + up * Vector1.Dot(up, Forward) * (1 - cos);

            Up = Vector1.Cross(Forward, Right).Normalize();
            Right = Vector1.Cross(Forward, new Vector1(0, -1, 0)).Normalize();
        }

        public void RotateUp(double v)
        {
            float sin = (float)Math.Sin(v);
            float cos = (float)Math.Cos(v);

            var nf = Forward * cos + Vector1.Cross(Right, Forward) * sin + Right * Vector1.Dot(Right, Forward) * (1 - cos);
            if (Math.Abs(nf.Y) > 0.99f) return;
            Forward = nf;

            Up = Vector1.Cross(Forward, Right).Normalize();
            Right = Vector1.Cross(Forward, new Vector1(0, -1, 0)).Normalize();
        }
    }
    public class Camera8
    {
        public Camera8(Vector8 pos, Vector8 forward, Vector8 up, Vector8 right)
        {
            Pos = pos;
            Forward = forward;
            Up = up;
            Right = right;
        }

        public Vector8 Pos;
        public Vector8 Forward;
        public Vector8 Up;
        public Vector8 Right;

        public void MoveForward(float delta)
        {
            Pos += Forward * new f8(delta);
        }
        public void MoveRight(float delta)
        {
            Pos += Right * new f8(delta);
        }


        public static Camera8 Create(Vector8 pos, Vector8 lookAt)
        {
            Vector8 forward = (lookAt - pos).Normalize();
            Vector8 down = new Vector8(0, -1, 0);

            Vector8 right = Vector8.Cross(forward, down).Normalize();
            Vector8 up = Vector8.Cross(forward, right).Normalize();

            return new Camera8(pos, forward, up, right);
        }


        public void RotateLeft(double v)
        {
            f8 sin = new f8((float)Math.Sin(v));
            f8 cos = new f8((float)Math.Cos(v));
            var up = new Vector8(0, 1, 0);
            Forward = Forward * cos + Vector8.Cross(up, Forward) * sin + up * Vector8.Dot(up, Forward) * (1 - cos);

            Up = Vector8.Cross(Forward, Right).Normalize();
            Right = Vector8.Cross(Forward, new Vector8(0, -1, 0)).Normalize();
        }

        public void RotateUp(double v)
        {
            f8 sin = new f8((float)Math.Sin(v));
            f8 cos = new f8((float)Math.Cos(v));

            var nf = Forward * cos + Vector8.Cross(Right, Forward) * sin + Right * Vector8.Dot(Right, Forward) * (1 - cos);
            if (Math.Abs(nf.Y.GetElement(0)) > 0.99f) return;
            Forward = nf;

            Up = Vector8.Cross(Forward, Right).Normalize();
            Right = Vector8.Cross(Forward, new Vector8(0, -1, 0)).Normalize();
        }
    }
}
