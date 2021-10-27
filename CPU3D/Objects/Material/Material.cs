using CPU3D.Draw;
using CPU3D.RayTrace;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Objects
{
    public class Material
    {
        public bool OnlyColor;
        public bool OnlyReflect;
        public bool OnlyDielectric;
        public bool OnlyGetColor;

        public bool HasShadow = true;
        public bool HasColoredShadow = false;
        public bool IsMouseHover;

        public float Optics_Absorptionrate;
        public float Optics_RefractionIndex;
        public float Reflective_Percentage;
        public Color Color;
        public Texture Texture;

        public static Material Realistic(Color c, float RefractionIndex) => new Material() { OnlyDielectric = true, Color = c, Optics_RefractionIndex = RefractionIndex };
        public static Material Textured(Texture texture, Func<HitInfo, Vector2> GetUV) => new Material() {  GetColor = (x) => texture.BilinearColorFromUV(GetUV(x)), Texture = texture };
        public static Material Simple(Color c) => new Material() { Color = c, OnlyColor = true, GetColor = (x) => c };
        public static Material ColorIsFunction(Func<HitInfo, Color> GetColor) => new Material() { OnlyGetColor = true, GetColor = GetColor };
        public static Material Reflect(Color c, float Reflectivity) => new Material() { OnlyReflect = true, Color = c, Reflective_Percentage = Reflectivity};
    
        public Func<HitInfo,Color> GetColor;
        
        public static Color CheckerBoardFunction(HitInfo hit)
        {
            Vector3 pos = hit.HitPos * (1/32.0f);
            int tmp = (int)(Math.Floor(pos.X) + Math.Floor(pos.Z));
            if ((tmp & 1) == 0) return (0.8f, 0.8f, 0.8f);
            return (0.1f, 0.1f, 0.1f);
        }

        static readonly Vector2 DotFive = new Vector2(0.5f);
        static readonly Vector2 Normalizer = new Vector2(1 / (2 * Math.PI), -1 / Math.PI);
        public static Vector2 SphericalTextureUV(HitInfo hit)
        {
            var pos = hit.HitPos;

            //float x = 0.5f + (float)(Math.Atan2(pos.Z, pos.X) / (2 * Math.PI));
            //float y = 0.5f + (float)(Math.Asin(pos.Y) / (-Math.PI));

            Vector2 xy = new Vector2(Math.Atan2(pos.Z, pos.X), Math.Asin(pos.Y));
            Vector2 res = DotFive + xy * Normalizer;
            return res;
            //return new Vector2(x, y);
        }
    }

}
