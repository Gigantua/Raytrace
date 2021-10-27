using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Algebra
{
    public class Matrix3
    {
        double[] Row1;
        double[] Row2;
        double[] Row3;

        public Matrix3()
        {
            Row1 = new double[3];
            Row2 = new double[3];
            Row3 = new double[3];
        }

        public double A { get => Row1[0]; set => Row1[0] = value; }
        public double B { get => Row1[1]; set => Row1[1] = value; }
        public double C { get => Row1[2]; set => Row1[2] = value; }
        public double D { get => Row2[0]; set => Row2[0] = value; }
        public double E { get => Row2[1]; set => Row2[1] = value; }
        public double F { get => Row2[2]; set => Row2[2] = value; }
        public double G { get => Row3[0]; set => Row3[0] = value; }
        public double H { get => Row3[1]; set => Row3[1] = value; }
        public double I { get => Row3[2]; set => Row3[2] = value; }

        public override string ToString()
        {
            return $"{String.Join("", Row1)}{Environment.NewLine}{String.Join("", Row2)}{Environment.NewLine}{String.Join("", Row3)}";
        }

        public static Matrix3 Identity()
        {
            Matrix3 id = new Matrix3();
            id[0, 0] = 1;
            id[1, 1] = 1;
            id[2, 2] = 1;
            return id;
        }

        public static Matrix3 Rotate(double angle)
        {
            double co = Math.Cos(angle);
            double si = Math.Sin(angle);

            Matrix3 rot = new Matrix3();
            rot[0, 0] = co;
            rot[0, 1] = -si;
            rot[1, 0] = si;
            rot[1, 1] = co;
            rot[2, 2] = 1;
            return rot;
        }

        public static Matrix3 Translate(double X, double Y)
        {
            Matrix3 rot = new Matrix3();
            rot[0, 2] = X;
            rot[1, 2] = Y;

            rot[2, 2] = 1;
            rot[0, 0] = 1;
            rot[1, 1] = 1;
            return rot;
        }

        public static Matrix3 RotateTranslate(double angle,double X,double Y)
        {
            double co = Math.Cos(angle);
            double si = Math.Sin(angle);

            Matrix3 rot = new Matrix3();
            rot[0, 0] = co;
            rot[0, 1] = -si;
            rot[0, 2] = X;
            rot[1, 0] = si;
            rot[1, 1] = co;
            rot[1, 2] = Y;
            rot[2, 2] = 1;
            return rot;
        }

        public static Matrix3 ShearXY(double delta_kx,double delta_ky)
        {
            Matrix3 shear = Matrix3.Identity();
            shear[0, 1] = delta_kx;
            shear[1, 0] = delta_ky;
            return shear;
        }

        public static Matrix3 ScaleXY(double kx, double ky)
        {
            Matrix3 scale = Matrix3.Identity();
            scale[0, 0] = kx;
            scale[1, 1] = ky;
            scale[2, 2] = 1;
            return scale;
        }


        public static Matrix3 RotateAroundTranslate(double RotationCenterX, double RotationCenterY, double angle, double dX, double dY)
        {
            double co = Math.Cos(angle);
            double si = Math.Sin(angle);

            Matrix3 negTranslate = Matrix3.Translate(-RotationCenterX, -RotationCenterY);
            Matrix3 Rotation = Matrix3.Rotate(angle);
            Matrix3 Translate = Matrix3.Translate(RotationCenterX + dX, RotationCenterY + dY);
            Matrix3 scale = Matrix3.ScaleXY(1,1);
            Matrix3 shear = Matrix3.ShearXY(0, 0);

            return shear * Translate * Rotation * negTranslate * scale;
        }

        public Matrix3 Inverse()
        {
            Matrix3 res = new Matrix3();

            res[0, 0] = E * I - F * H;
            res[0, 1] = -(B * I - C * H);
            res[0, 2] = B * F - C * E;
            res[1, 0] = -(D * I - F * G);
            res[1, 1] = A * I - C * G;
            res[1, 2] = -(A * F - C * D);
            res[2, 0] = D * H - E * G;
            res[2, 1] = -(A * H - B * G);
            res[2, 2] = A * E - B * D;

            return res * (1/Det);
        }


        public double Det
        {
            get
            {
                return (A * E * I + B * F * G + C * D * H) - (B * D * I + A * F * H + C * E * G);
            }
        }

        public void OnRow(int Row, Func<double,double> f)
        {
            switch (Row)
            {
                case 0:
                    Row1[0] = f(Row1[0]);
                    Row1[1] = f(Row1[1]);
                    Row1[2] = f(Row1[2]);
                    break;
                case 1:
                    Row2[0] = f(Row2[0]);
                    Row2[1] = f(Row2[1]);
                    Row2[2] = f(Row2[2]);
                    break;
                case 2:
                    Row2[0] = f(Row2[0]);
                    Row2[1] = f(Row2[1]);
                    Row2[2] = f(Row2[2]);
                    break;
                default: throw new ArgumentOutOfRangeException("Row is not between 1 and 3");
            }
        }

        public void OnAllElements(Func<double,double> f)
        {
            Parallel.For(0, 3, (i) =>
            {
                OnRow(i, f);
            });
        }


        public Matrix3 Clone()
        {
            Matrix3 clone = new Matrix3();
            Array.Copy(this.Row1, clone.Row1, 3);
            Array.Copy(this.Row2, clone.Row2, 3);
            Array.Copy(this.Row3, clone.Row3, 3);
            return clone;
        }

        public static Matrix3 operator *(Matrix3 m1, double c)
        {
            Matrix3 ret = m1.Clone();
            ret.OnAllElements(x => x * c);
            return ret;
        }

        public static Matrix3 operator +(Matrix3 m1, Matrix3 m2)
        {
            Matrix3 ret = m1.Clone();

            ret.Row1[0] += m2.Row1[0];
            ret.Row1[1] += m2.Row1[1];
            ret.Row1[2] += m2.Row1[2];

            ret.Row2[0] += m2.Row2[0];
            ret.Row2[1] += m2.Row2[1];
            ret.Row2[2] += m2.Row2[2];

            ret.Row3[0] += m2.Row3[0];
            ret.Row3[1] += m2.Row3[1];
            ret.Row3[2] += m2.Row3[2];

            return ret;
        }

        public static Vector3 operator *(Matrix3 m1,Vector3 v1)
        {
            Vector3 result = new Vector3();

            result.X = m1.Row1[0] * v1.X + m1.Row1[1] * v1.Y + m1.Row1[2] * v1.Z;
            result.Y = m1.Row2[0] * v1.X + m1.Row2[1] * v1.Y + m1.Row2[2] * v1.Z;
            result.Z = m1.Row3[0] * v1.X + m1.Row3[1] * v1.Y + m1.Row3[2] * v1.Z;

            return result;
        }

        public (double X, double Y) Fast2DMultiply(double X, double Y)
        {
            double XR = this.Row1[0] * X + this.Row1[1] * Y + this.Row1[2];
            double YR = this.Row2[0] * X + this.Row2[1] * Y + this.Row2[2];
            return (XR, YR);
        }
        


        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            Matrix3 result = new Matrix3();

            for (int x = 0; x < 3; x++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0;
                    for (int y = 0; y < 3; y++)
                    {
                        sum += m1[x, y] * m2[y, j];
                    }
                    result[x, j] = sum;
                }
            }

            return result;
        }

        public double this[int Row,int Column]
        {
            get
            {
                switch(Row)
                {
                    case 0:
                        return Row1[Column];
                    case 1:
                        return Row2[Column];
                    case 2:
                        return Row3[Column];
                    default: throw new ArgumentOutOfRangeException("Row is not between 1 and 3");
                }
            }
            set
            {
                switch (Row)
                {
                    case 0:
                        Row1[Column] = value;
                        break;
                    case 1:
                        Row2[Column] = value;
                        break;
                    case 2:
                        Row3[Column] = value;
                        break;
                    default: throw new ArgumentOutOfRangeException("Row is not between 1 and 3");
                }
            }
        }
    }
}
