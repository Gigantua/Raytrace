using CPU3D.Draw;
using CPU3D.RayTrace;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPU3D.Objects
{
    public class Scene
    {
        public List<Object3D> Objects = new List<Object3D>();
        public List<Light> Lights = new List<Light>();
        public float AmbientLight = 0.3f;
        public long RayCount;

        List<Camera> Cameras = new List<Camera>();
        Sphere Skysphere = new Sphere(new Vector3(0, 0, 0), 100000);

        public Camera cam => Cameras[CamIndex];

        int CamIndex;
        public void SetCamera(int index)
        {
            CamIndex = index;
        }

        public Scene()
        {
            Cameras.Add(new Camera(new Vector3(112, 50, 55), new Vector3(-0.91f, -0.322f, -0.477f), 80));
            
            Skysphere.Material = Material.Textured(new Texture("Skybox.jpg"), Material.SphericalTextureUV);
            //Skysphere.Material = Material.Simple((1, 1, 1));
            Lights.Add(new Light(200, 400, 200));
        }


        public void RemoveObject(Object3D b)
        {
            Objects.Remove(b);
        }

        public T AddObject<T>(T obj) where T : Object3D
        {
            Objects.Add(obj);
            return obj;
        }

        public void ClearHover()
        {
            int c = Objects.Count;
            for (int i = 0; i < c; i++)
            {
                Objects[i].Material.IsMouseHover = false;
            }
        }

        
        public static int MaxDepth = 8;
        static readonly float CTMin = 0.02f; //Amount of Color at which ray will not be calculated
        static readonly float CTMinSquared = CTMin * CTMin; //Amount of Color at which ray will not be calculated
        static readonly float CTMax = 0.98f; //Amount of Color at which ray will not be calculated

        public Color CastRay(Ray r, out HitInfo objectHit)
        {
            if (!Collideray(r, out objectHit)) return Skysphere.Material.GetColor(new HitInfo(r)); //Hit Sky
            var material = objectHit.HitObj.Material;

            Color objcolor;
            if (!objectHit.InsideObject)
            {
                objcolor = CastSceneRay(r, objectHit, 0);
            } 
            else
            {
                if (material.OnlyDielectric) objcolor = ExitObject(r.ChangeRefraction(material.Optics_RefractionIndex), objectHit.HitObj, objectHit, 0);
                else if (material.OnlyReflect) objcolor = (1 - material.Reflective_Percentage) * material.Color;
                else if (material.OnlyColor) objcolor = material.Color;
                else objcolor = material.GetColor(objectHit);
            }
            if (material.IsMouseHover) return 0.3f * Colors.White + 0.7f * objcolor; //Mouse selection

            return objcolor;
        }


        Color CastSceneRay(Ray r, HitInfo Collision, int depth)
        {
            //If No Collision try to get target
            if (Collision == null && !Collideray(r, out Collision)) return Skysphere.Material.GetColor(new HitInfo(r)); //Hit Sky

            var material = Collision.HitObj.Material;
            if (depth > MaxDepth) return material.Color;

            Color SceneColor;
            if (material.OnlyColor) SceneColor = material.Color;
            else if (material.OnlyReflect)
            {
                if (material.Reflective_Percentage < CTMin) SceneColor = material.Color;
                else if (material.Reflective_Percentage > CTMax) SceneColor = ReflectRay(r, Collision, depth + 1);
                else SceneColor = Color.Mix(material.Color, ReflectRay(r, Collision, depth + 1), material.Reflective_Percentage);
            }
            else if (material.OnlyDielectric)
            {
                SceneColor = EnterObject(r, Collision, depth + 1);
            }
            else if (material.OnlyGetColor)
            {
                SceneColor = material.GetColor(Collision);
            }
            else return Colors.None;

            //SceneColor is a valid hit with any object
            if (material.HasShadow && !SceneColor.IsNan) FastShadow(ref SceneColor, Collision); //Make Shadow if we are not inside obj

            return SceneColor;
        }

        /// <summary>
        /// Transparent Materials also block light.
        /// </summary>
        void FastShadow(ref Color SceneColor, HitInfo Collision)
        {
            if (Collision.InsideObject) return;

            Vector3 shadowdir = Lights[0].Position - Collision.HitPos;

            Ray RayToLight = new Ray(Collision.HitPos, shadowdir, 0.002f, 1);
            float LightDist = shadowdir.LengthSquared();

            if (Collideray(RayToLight, out HitInfo lighthit)) //Full Shadow
            {
                if (lighthit.dist * lighthit.dist > LightDist) goto InLight; //We hit object behind lightsource -> Direct lit

                SceneColor *= AmbientLight;
                return;
            }

            InLight:
            Vector3 dir = Vector3.Normalize(shadowdir);
            float n = Vector3.Dot(dir, Collision.Normal) * (1 - AmbientLight) + AmbientLight; //Map range 0 -> 1 to Ambient -> 1
            if (n > 1) return; //No need to multiply with 1

            SceneColor *= Math.Max(n, AmbientLight); //Clamped to Ambient...1
        }


        void GetShadow(ref Color SceneColor, HitInfo Collision)
        {
            Vector3 shadowdir = Lights[0].Position - Collision.HitPos;
            Ray RayToLight = new Ray(Collision.HitPos, shadowdir).Offset(0.002f);
            float LightDist = shadowdir.Length();

            bool first = true;
            int loopcount = 0;

            while (Collideray(RayToLight, out HitInfo Occlusion) && Occlusion.dist < LightDist) //Collide ray until we surpass light
            {
                if (loopcount > MaxDepth) break;
                if (!Occlusion.HitObj.Material.HasShadow) break; //Goto is ugly.
                if (Occlusion.InsideObject && first) break; //We start inside an object = we are on surface
                if (!Occlusion.HitObj.Material.OnlyDielectric) //We hit a non dielectric so we are in full shadow
                {
                    SceneColor *= AmbientLight;
                    return;
                }
                if (Occlusion.InsideObject) //We entered a dielectric object so we walk to exit
                {
                    Object3D Entry = Occlusion.HitObj;
                    if (!Collideray(RayToLight, Occlusion.HitObj, out HitInfo ExitInfo)) { SceneColor *= AmbientLight; return; } //Invalid so we are in full shadow
                    
                    if (Occlusion.HitObj.Material.HasColoredShadow)
                    {
                        if (ExitInfo.dist < 0.01f) goto MoveRay; //We only brush the object
                        Color Obj = Colors.White - Entry.Material.Color;
                        Color TranslucentColor = Color.Exp(Obj, -ExitInfo.dist * Entry.Material.Optics_Absorptionrate);
                        if (TranslucentColor.LumaSquared < CTMinSquared) { SceneColor *= AmbientLight; return; } //Full absorption so we are in shadow

                        SceneColor = Color.Mix(SceneColor * TranslucentColor, SceneColor, AmbientLight);
                    }
                    else //We hit uncolored object
                    {
                        SceneColor *= AmbientLight;
                        return;
                    }
                    Occlusion = ExitInfo; //Get right new ray start at end of loop
                }
                
                MoveRay:
                first = false;
                RayToLight = RayToLight.Offset(Occlusion.dist + 0.002f);
                LightDist = Vector3.Distance(Lights[0].Position, Occlusion.HitPos);
                loopcount++;
                continue;
            }

            Vector3 lightnormal = Collision.Normal;
            Vector3 dir = Vector3.Normalize(Lights[0].Position - Collision.HitPos);

            float n = Vector3.Dot(dir, lightnormal) * (1 - AmbientLight) + AmbientLight; //Map range 0 -> 1 to Ambient -> 1
            if (n < AmbientLight) n = AmbientLight;
            if (n > 1) n = 1;
            SceneColor *= n;
        }


        
        Color EnterObject(Ray ray, HitInfo SurfaceHit, int depth)
        {
            var material = SurfaceHit.HitObj.Material;
            if (depth > MaxDepth) return material.Color;

            FresnelEquation e = new FresnelEquation(ray, material.Optics_RefractionIndex, SurfaceHit.Normal, SurfaceHit.HitPos);

            if (e.R > CTMax) return CastSceneRay(e.ReflectRay, null, depth + 1); //Total reflection
            if (e.T > CTMax) return ExitObject(e.RefractRay, SurfaceHit.HitObj, null, depth + 1); //Total refraction into obj

            Color Outside = CastSceneRay(e.ReflectRay, null, depth + 1);
            Color Inside = ExitObject(e.RefractRay, SurfaceHit.HitObj, null, depth + 1);

            return Inside * e.T + Outside * e.R;
        }


        Color ExitObject(Ray ray, Object3D Inside, HitInfo ExitHit, int depth)
        {
            if (ExitHit == null && !Collideray(ray, Inside, out ExitHit))//Ray started inside object and did not hit any surface? -> We hit a very thin edge
            {
                if (depth != 0) return Inside.Material.Color; //Not the first bounce
                return Colors.None;
            } 
            var material = Inside.Material;
            if (depth > MaxDepth) return material.Color;

            Color Obj = Colors.White - material.Color;
            Color TranslucentColor = Color.Exp(Obj, -ExitHit.dist * material.Optics_Absorptionrate);
            if (TranslucentColor.LumaSquared < CTMinSquared) return TranslucentColor; //100% absorption

            FresnelEquation e = new FresnelEquation(ray, 1, ExitHit.Normal, ExitHit.HitPos);
        
            if (e.R > CTMax) return TranslucentColor * TotalReflection(e.ReflectRay, Inside, depth + 1); //Total reflection
            if (e.T > CTMax) return TranslucentColor * CastSceneRay(e.RefractRay, null, depth + 1); //Total refraction

            Color InsideColor = e.R * TotalReflection(e.ReflectRay, Inside, depth + 1); 
            Color Outside = e.T * CastSceneRay(e.RefractRay, null, depth + 1);

            return TranslucentColor * (InsideColor + Outside);
        }


        //Let Ray hit and continue same path
        Color TotalReflection(Ray ray, Object3D Inside, int depth)
        {
            //Ray started inside object and did not hit any surface?
            //Normal has a hard jump (edges). The Fresnelequation pushes the ray outside
            //Dist is big so ray.walk is still outside
            if (!Collideray(ray, Inside, out HitInfo ExitHit)) return Inside.Material.Color;
            if (ExitHit.HitObj != Inside) return Inside.Material.Color;
            
            Material material = Inside.Material;
            Color MaterialColor = material.Color;
            if (depth > MaxDepth) return MaterialColor;

            Color Inverse = Colors.White - MaterialColor;

            Color TranslucentColor = Color.Exp(Inverse, -ExitHit.dist * material.Optics_Absorptionrate);
            if (TranslucentColor.LumaSquared < CTMinSquared) return TranslucentColor; //100% absorption

            FresnelEquation e = new FresnelEquation(ray, material.Optics_RefractionIndex, ExitHit.Normal, ExitHit.HitPos);
            if (e.R > CTMax) return TranslucentColor * MaterialColor; //2x Total internal reflection = color (good approximation)
            
            return e.T * TranslucentColor * CastSceneRay(e.RefractRay, null, depth + 1);
        }

        Color ReflectRay(Ray r, HitInfo objectHit, int depth)
        {
            Ray reflectedray = FresnelEquation.ReflectSurface(r, objectHit.HitPos, objectHit.Normal);
            return CastSceneRay(reflectedray, null, depth + 1);
        }
        

        public bool CastSceneRay(Ray r, out HitInfo info)
        {
            Ray l = new Ray(r.Origin, r.Direction);
            return Collideray(l, out info);
        }

        bool Collideray(Ray r, out HitInfo hitinfo)
        {
            //Interlocked.Increment(ref RayCount);
            hitinfo = RayHelper.CollideRay(r);
            return hitinfo != null;
        }

        bool Collideray(Ray r, Object3D inside, out HitInfo hitinfo)
        {
            //Interlocked.Increment(ref RayCount);
            hitinfo = inside.Hittest(r);
            return hitinfo != null;
        }


        Octree RayHelper;
        /// <summary>
        /// Builds Octree for faster Raycastion
        /// </summary>
        public void FrameStart(bool Changed = false)
        {
            if (Changed == false && RayHelper != null) return;

            RayHelper = new Octree(-3000 * Vector3.One, 3000 * Vector3.One);
            RayHelper.Split(Objects, 9);

            Console.WriteLine($"Octree Boxes: {RayHelper.GetTotalBoxcount()}");
            Console.WriteLine($"Total Items: {Objects.Count}");
        }

        
        
        

    }
}
