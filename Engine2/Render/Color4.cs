using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Render
{
    public static class Colors
    {
        public static Color4 Transparent => Color4.FromElements(0, 0, 0, 0);
        public static Color4 Black => Color4.FromElements(0, 0, 0, 1);
        public static Color4 Red => Color4.FromElements(1, 0, 0, 1);
        public static Color4 Green => Color4.FromElements(0, 1, 0, 1);
        public static Color4 Blue => Color4.FromElements(0, 0, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Color4
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public static Color4 FromElements(float R,float G,float B,float A)
        {
            return new Color4() { R = R, G = G, B = B, A = A };
        }

        public static Color4 operator *(Color4 color, float f)
        {
            return new Color4()
            {
                R = color.R * f,
                G = color.G * f,
                B = color.B * f,
                A = color.A
            };
        }

        public static Color4 operator +(Color4 other,Color4 self)
        {
            other.A = 0.5f;
            self.A = 0.5f;

            float aA = self.A;
            float gA = self.G;
            float bA = self.B;
            float rA = self.R;

            float aB = other.A;
            float gB = other.G;
            float bB = other.B;
            float rB = other.R;

            float rOut = (rA * aA) + (rB * aB * (1 - aA));
            float gOut = (gA * aA) + (gB * aB * (1 - aA));
            float bOut = (bA * aA) + (bB * aB * (1 - aA));
            float aOut = aA + (aB * (1 - aA));

            return new Color4() { R = rOut, G = gOut, B = bOut, A = aOut };
        }

        public static Color4 Add(float Brightness, Color4 A,Color4 B)
        {
            Brightness = Math.Min(1, Brightness);
            return A * Brightness + B * Brightness;
        }
        public static Color4 Add(float Brightness, Color4 A, Color4 B, Color4 C)
        {
            Brightness = Math.Min(1, Brightness);
            return A * Brightness + B * Brightness + C * Brightness;
        }


        static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static Color4 Clamp(Color4 value)
        {
            return Color4.FromElements(Clamp(value.R, 0, 1), Clamp(value.G, 0, 1), Clamp(value.B, 0, 1), Clamp(value.A, 0, 1));
        }
    }

    public static class Colorconvert
    {
        public static Color4 FromColor(System.Drawing.Color color)
        {
            return new Color4() { R = color.R / 255.0f, G = color.G / 255.0f, B = color.B / 255.0f, A = color.A / 255.0f };
        }

    }
}
