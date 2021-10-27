using Engine.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Algebra;
using System.ComponentModel;

namespace Engine
{
    public abstract class Object2D
    {
        bool Dirty = true;
        Point2D loc, rotc;
        double rotangle;
        public Point2D Location
        {
            get
            {
                return loc;
            }
            set
            {
                if (loc != value)
                {
                    loc = value;
                    Dirty = true;
                }
            }
        }
        public Point2D RotationCenter
        {
            get
            {
                return rotc;
            }
            set
            {
                if(rotc != value)
                {
                    Dirty = true;
                    rotc = value;
                }
            }
        }
        public double RotationAngle
        {
            get
            {
                return rotangle;
            }
            set
            {
                if (rotangle != value)
                {
                    Dirty = true;
                    rotangle = value;
                }
            }
        }

        Matrix3 transform;
        public Matrix3 Transform
        {
            get
            {
                if (Dirty || transform == null)
                {
                    transform = Matrix3.RotateAroundTranslate(RotationCenter.X, RotationCenter.Y, RotationAngle, Location.X, Location.Y);
                    Dirty = false;
                }
                return transform;
            }
            set
            {
                transform = value;
            }
        }

        Matrix3 inversetransform;
        public Matrix3 InverseTransform
        {
            get
            {
                if (Dirty || inversetransform == null)
                {
                    inversetransform = Matrix3.RotateAroundTranslate(RotationCenter.X, RotationCenter.Y, RotationAngle, Location.X, Location.Y).Inverse();
                    Dirty = false;
                }
                return inversetransform;
            }
            set
            {
                inversetransform = value;
            }
        }

        /// <summary>
        /// GetBounds
        /// </summary>
        public Rectangle2D TransformedBounds()
        {
            Rectangle2D res = new Rectangle2D();

            res.UPLEFT = TransformRotate(0, 0);
            res.UPRIGHT = TransformRotate(WidthHeight.dX, 0);
            res.DOWNLEFT = TransformRotate(0, WidthHeight.dY);
            res.DOWNRIGHT = TransformRotate(WidthHeight.dX, WidthHeight.dY);

            Dirty = true;
            return res;
        }

        public Vector2D WidthHeight { get; set; }

        public double Width => WidthHeight.dX;
        public double Height => WidthHeight.dY;

        public virtual void Render(DirectBitmap Target,int ScreenX,int ScreenY)
        {
            var (LocalX, LocalY) = this.InverseTransformRotate(ScreenX, ScreenY);
            double width = this.Width;
            double height = this.Height;

            if (LocalX < 0 || LocalX >= width) return;
            if (LocalY < 0 || LocalY >= height) return;

            _Render(Target, ScreenX, ScreenY, LocalX, LocalY);
        }

        /// <summary>
        /// Transforms screen to local coordinates. Is only called if LocalX and LocalY is inside Width and Height
        /// </summary>
        protected abstract void _Render(DirectBitmap Target, int ScreenX, int ScreenY, double LocalX, double LocalY);

        public Object2D()
        {
            Location = new Point2D();
        }

        /// <summary>
        /// Rotates relative pixel of image
        /// </summary>
        public Point2D TransformRotate(double XPixel, double YPixel)
        {
            var result = Transform * new Engine.Algebra.Vector3(XPixel,YPixel,1);
            return new Point2D(result.X,result.Y);
        }


        /// <summary>
        /// Translates image pixel to object pixel
        /// </summary>
        public (double X,double Y) InverseTransformRotate(double BufferX, double BufferY)
        {
            return InverseTransform.Fast2DMultiply(BufferX, BufferY);
        }

        public Rectangle2D InverseTransformRotateRectangle()
        {
            var A = TransformRotate(0, 0);
            var B = TransformRotate(WidthHeight.dX, 0);
            var C = TransformRotate(0, WidthHeight.dY);
            var D = TransformRotate(WidthHeight.dX, WidthHeight.dY);

            return new Rectangle2D();
        }
    }

    public static class DrawShared
    {
        public static Point2D ProjectToScreen(Point3D Target, Camera3D Camera, Point3D ViewerPosition)
        {
            double cx = Math.Cos(Camera.Orientation.dX);
            double sx = Math.Sin(Camera.Orientation.dX);

            double cy = Math.Cos(Camera.Orientation.dY);
            double sy = Math.Sin(Camera.Orientation.dY);

            double cz = Math.Cos(Camera.Orientation.dZ);
            double sz = Math.Sin(Camera.Orientation.dZ);

            double x = Target.X - Camera.Position.X;
            double y = Target.Y - Camera.Position.Y;
            double z = Target.Z - Camera.Position.Z;

            double dx = cy * (sz * y + cz * x) - sy * z;
            double dy = sx * (cy * z + sy * (sz * y + cz * x)) + cx * (cz * y - sz * x);
            double dz = cx * (cy * z + sy * (sz * y + cz * x)) - sx * (cz * y - sz * x);

            double bx = ViewerPosition.Z * dx / dz - ViewerPosition.X;
            double by = ViewerPosition.Z * dy / dz - ViewerPosition.Y;

            return new Point2D(bx, by);
        }

        
    }
}
