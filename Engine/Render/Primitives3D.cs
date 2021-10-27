using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Point3D
    {
        public double X;

        public double Y;

        public double Z;

        public Point3D(double X,double Y,double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Point3D Copy()
        {
            return new Point3D(this.X, this.Y, this.Z);
        }

        public static Point3D Zero
        {
            get { return new Point3D(); }
        }

        public static Vector3D operator -(Point3D End, Point3D Start)
        {
            return new Vector3D(End.X - Start.X, End.Y - Start.Y, End.Z - Start.Z);
        }

        public Point3D Move(double dX, double dY, double dZ)
        {
            this.X += dX;
            this.Y += dY;
            this.Z += dZ;
            return this;
        }

        public override string ToString()
        {
            return $"({X}|{Y}|{Z})";
        }
    }

    public struct Vector3D
    {
        public double dX;

        public double dY;

        public double dZ;

        public Vector3D(double dX, double dY, double dZ) : this()
        {
            this.dX = dX;
            this.dY = dY;
            this.dZ = dZ;
        }

        public double Length
        {
            get { return Math.Sqrt(dX * dX + dY * dY + dZ * dZ); }
        }

        public void Normalize()
        {
            throw new NotImplementedException();
        }

        public static Vector3D UnitX
        {
            get { return new Vector3D(1, 0, 0); }
        }
        public static Vector3D UnitY
        {
            get { return new Vector3D(0, 1, 0); }
        }
        public static Vector3D UnitZ
        {
            get { return new Vector3D(0, 0, 1); }
        }
    }

    public struct Ray3D
    {
        public Vector3D Direction;
        public Point3D Origin;

        public Ray3D(Point3D Origin, Vector3D Direction)
        {
            this.Direction = Direction;
            this.Origin = Origin;
        }

        public Point3D Walk(double dist)
        {
            return new Point3D(Origin.X + Direction.dX * dist, Origin.Y + Direction.dY * dist, Origin.Z + Direction.dZ * dist);
        }


        public static Ray3D UnitX
        {
            get { return new Ray3D(Point3D.Zero, Vector3D.UnitX); }
        }

        public static Ray3D UnitY
        {
            get { return new Ray3D(Point3D.Zero, Vector3D.UnitY); }
        }

        public static Ray3D UnitZ
        {
            get { return new Ray3D(Point3D.Zero, Vector3D.UnitZ); }
        }
    }
}
