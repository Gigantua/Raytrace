using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Point2D
    {
        public double X, Y;

        public Point2D(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Point2D Copy()
        {
            return new Point2D(this.X, this.Y);
        }

        public static Point2D Zero
        {
            get { return new Point2D(); }
        }

        public static Vector2D operator -(Point2D End, Point2D Start)
        {
            return new Vector2D(End.X - Start.X, End.Y - Start.Y);
        }

        public Point2D Move(double dX, double dY)
        {
            this.X += dX;
            this.Y += dY;
            return this;
        }

        public override string ToString()
        {
            return $"({X}|{Y})";
        }


        #region Equals
        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            if(obj is Point2D p)
            {
                return p.X == X && p.Y == Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        #endregion
    }

    public struct Vector2D
    {
        public double dX, dY;

        public Vector2D(double dX, double dY)
        {
            this.dX = dX;
            this.dY = dY;
        }

        public Vector2D(double Angle)
        {
            this.dX = Math.Cos(Angle);
            this.dY = Math.Sin(Angle);
            this.Normalize();
        }

        public static Vector2D Zero
        {
            get { return new Vector2D(); }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(dX * dX + dY * dY);
            }
        }

        public void Normalize()
        {
            double dr = Length;
            this.dX = dX / dr;
            this.dY = dY / dr;
        }
    }

    public struct Rectangle2D
    {
        public Point2D Position;
        public Vector2D Size;

        public Point2D UPLEFT;
        public Point2D UPRIGHT;
        public Point2D DOWNLEFT;
        public Point2D DOWNRIGHT;

        Nullable<Point2D> max;
        public Nullable<Point2D> Max
        {
            get
            {
                if (max == null)
                {
                    max = new Point2D(calcmax(UPLEFT.X, UPRIGHT.X, DOWNLEFT.X, DOWNRIGHT.X), calcmax(UPLEFT.Y, UPRIGHT.Y, DOWNLEFT.Y, DOWNRIGHT.Y));
                }
                return max;
            }
        }

        Nullable<Point2D> min;
        public Nullable<Point2D> Min
        {
            get
            {
                if (min == null)
                {
                    min = new Point2D(calcmin(UPLEFT.X, UPRIGHT.X, DOWNLEFT.X, DOWNRIGHT.X), calcmin(UPLEFT.Y, UPRIGHT.Y, DOWNLEFT.Y, DOWNRIGHT.Y));
                }
                return min;
            }
        }

        static double calcmax(params double[] values)
        {
            return Enumerable.Max(values);
        }

        static double calcmin(params double[] values)
        {
            return Enumerable.Min(values);
        }

        public bool Inside(double x,double y)
        {
            if (Min.Value.X > x || x > Max.Value.X) return false;
            if (Min.Value.Y > y || y > Max.Value.Y) return false;
            return true;
        }
    }

    public struct Ray2D
    {
        public Vector2D Direction;
        public Point2D Origin;

        public Ray2D(Point2D Origin,Vector2D Direction)
        {
            this.Direction = Direction;
            this.Origin = Origin;
        }

        public Point2D Walk(double dist)
        {
            return new Point2D(Origin.X + Direction.dX * dist, Origin.Y + Direction.dY * dist);
        }
    }

}
