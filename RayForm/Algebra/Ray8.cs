using System;
using System.Collections.Generic;
using System.Text;

namespace RayForm
{
    public readonly struct Ray8
    {
        public readonly Vector8 Origin;
        public readonly Vector8 Direction;
        //public readonly Vector8 InvDirection;

        public Ray8(Vector8 start, Vector8 dir)
        {
            Origin = start;
            Direction = dir;
            //InvDirection = dir.Invert;
        }

        public Vector8 Walk(f8 d) => Vector8.MultiplyAdd(d, Direction, Origin);
    }
}
