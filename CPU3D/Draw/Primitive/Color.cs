using System;
using System.Collections.Generic;
using System.Linq;
using CPU3D.RayTrace;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Draw
{
    static class Colors
    {
        public static readonly Color Red =      (1, 0, 0);
        public static readonly Color Green =    (0, 1, 0);
        public static readonly Color Blue =     (0, 0, 1);
        public static readonly Color White =    (1, 1, 1);
        public static readonly Color Black =    (0, 0, 0);
        public static readonly Color Yellow =   (1, 1, 0);
        public static readonly Color None = (float.NaN, 0, 0);

        /// <summary>
        /// Returns a random Color
        /// </summary>
        public static Color Random => ((float)Rand.NextDouble(), (float)Rand.NextDouble(), (float)Rand.NextDouble());

        public static Random Rand = new Random();


        public static Color8 Convert8Bit(Color c)
        {
            return ((byte)(c.R * 255), (byte)(c.G * 255), (byte)(c.B * 255));
        }

        public static Color Convertfloat(Color8 c)
        {
            return (c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
        }
    }

    public readonly struct Color : IColor<float,Vector3>, IEquatable<Color>
    {
        static Color LumaTransform = (0.299f, 0.587f, 0.114f);
        public readonly Vector3 RGB { get;  }
        public float R => RGB.X;
        public float G => RGB.Y;
        public float B => RGB.Z;
        public float A => 1;

        public static readonly Color White = (1, 1, 1);
        public static readonly Color Black = (0, 0, 0);


        public Color(Vector3 Elements)
        {
            RGB = Elements;
        }


        /// <summary>
        /// Performs linear weighted interpolation
        /// </summary>
        /// <param name="a">Color a</param>
        /// <param name="b">Color b</param>
        /// <param name="amount">A value between 0 and 1 that indicates the weight of b.</param>
        /// <returns></returns>
        public static Color Mix(Color a, Color b, float amount) => new Color(Vector3.Lerp(a.RGB, b.RGB, amount));

        /// <summary>
        /// Return RGB = e^(expotent * R), e^(exponent * G) e^(exponent * B)
        /// </summary>
        public static Color Exp(Color a, float exponent)
        {
            Vector3 exponents = exponent * a.RGB; //This is equal to (1+x/6)^6 which has a max abs. error of 0.04 for all negative values which is not visible
            
            Vector3 x = Vector3.One + exponents / 64.0f;
            x *= x; x *= x; x *= x; x *= x; x *= x; x *= x;
            return new Color(x);
        }


        public int ToInt()
        {
            throw new InvalidOperationException("4 floats do not fit into one integer");
        }

        public static implicit operator Vector3(Color d)
        {
            return d.RGB;
        }

        public static implicit operator Color(ValueTuple<float,float,float> rgb) => new Vector3(rgb.Item1, rgb.Item2, rgb.Item3);

        public static implicit operator Color(Vector3 rgb) => new Color(rgb);
        public static Color operator *(Color a, Color b) => new Color(a.RGB * b.RGB);
        public static Color operator *(float a, Color b) => new Color(a * b.RGB);
        public static Color operator *(Color a, float b) => new Color(b * a.RGB);
        public static Color operator +(Color a, Color b) => new Color(a.RGB + b.RGB);
        public static Color operator -(Color a, Color b) => new Color(a.RGB - b.RGB);

        public override bool Equals(object obj) => Vector3.Equals(this, obj);

        public bool Equals(Color other) => R == other.R && G == other.G && B == other.B;

        public override int GetHashCode() => RGB.GetHashCode();

        public float Luma => (float)Math.Sqrt(Vector3.Dot(RGB, LumaTransform));
        public float LumaSquared => Vector3.Dot(RGB, LumaTransform);
        public bool IsNan => R != R;

    }
}
