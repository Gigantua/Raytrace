using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using CPU3D.Objects;


namespace CPU3D.RayTrace
{
    public static partial class HitTest
    {
        
        public static void RayIntersectsAABBN(Span<float> Distances, Vector3[] boxmins, Vector3[] boxmaxs, Ray r)
        {
            Vector3 td1 = (boxmins[0] - r.Origin) * r.InverseDirection;
            Vector3 td2 = (boxmaxs[0] - r.Origin) * r.InverseDirection;

            Vector3 tdmin = Vector3.Min(td1, td2);
            Vector3 tdmax = Vector3.Max(td1, td2);

            for(int i = 0; i < Distances.Length; i++)
            {
                float tmax = tdmax.Min();
                if (tmax < 0) Distances[i] = float.PositiveInfinity;
                else
                {
                    float tmin = tdmin.Max();

                    if (tmax < tmin) Distances[i] = float.PositiveInfinity; //Behind ray
                    else if (tmin < 0) Distances[i] = 0;
                    else Distances[i] = tmax;
                }
            }
        }

    }
}
