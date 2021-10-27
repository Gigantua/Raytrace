using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RayNet
{
    public class Camera
    {
        public Camera(Vector3 pos, Vector3 forward, Vector3 up, Vector3 right)
        {
            Pos = pos;
            Forward = forward;
            Up = up;
            Right = right;
        }

        public Vector3 Pos;
        public Vector3 Forward;
        public Vector3 Up;
        public Vector3 Right;

        public void MoveForward(float delta)
        {
            Pos += Forward * delta;
        }
        public void MoveRight(float delta)
        {
            Pos += Right * delta;
        }

        public static Camera Create(Vector3 pos, Vector3 lookAt)
        {
            Vector3 forward = Vector3.Normalize((lookAt - pos));
            Vector3 down = new Vector3(0, -1, 0);

            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, down));
            Vector3 up = Vector3.Normalize(Vector3.Cross(forward, right));
            return new Camera(pos, forward, up, right);
        }


        public void RotateLeft(double v)
        {
            float sin = (float)Math.Sin(v);
            float cos = (float)Math.Cos(v);
            var up = new Vector3(0, 1, 0);
            Forward = Forward * cos + Vector3.Cross(up, Forward) * sin + up * Vector3.Dot(up, Forward) * (1 - cos);

            Up = Vector3.Normalize(Vector3.Cross(Forward, Right));
            Right = Vector3.Normalize(Vector3.Cross(Forward, new Vector3(0, -1, 0)));
        }

        public void RotateUp(double v)
        {
            float sin = (float)Math.Sin(v);
            float cos = (float)Math.Cos(v);

            var nf = Forward * cos + Vector3.Cross(Right, Forward) * sin + Right * Vector3.Dot(Right, Forward) * (1 - cos);
            if (Math.Abs(nf.Y) > 0.99f) return;
            Forward = nf;

            Up = Vector3.Normalize(Vector3.Cross(Forward, Right));
            Right = Vector3.Normalize(Vector3.Cross(Forward, new Vector3(0, -1, 0)));
        }
    }
}
