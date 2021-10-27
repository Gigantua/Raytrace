using CPU3D.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPU3D.RayTrace
{
    public class Octree
    {
        AABB Master;
        Vector3 min;
        Vector3 max;
        public List<Object3D> AllObjects { get; private set; }
        
        public Octree(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HitInfo CollideRay(Ray r)
        {
            //if (Master.ChildCount == 0)
            return GetHit(r, AllObjects); //May contain more than master if box.contains does not implement it
            //return Collide(r, Master);
        }

        int MinIndexExcept0(Span<float> distances)
        {
            float min = float.PositiveInfinity; 
            int minindex = 0;
            for(int i=0;i<distances.Length;i++)
            {
                if (distances[i] == 0) continue; //0 is already handled
                if (distances[i] < min) { min = distances[i]; minindex = i; }
            }
            return minindex;
        }

        HitInfo Collide(Ray r, AABB box)
        {
            int len = box.ChildCount;
            if (len == 0) return GetHit(r, box.ItemsInside); //No Children to check

            List<AABB> Children = box.Children;
            Span<float> HitDists = stackalloc float[len];

            //HitTest.RayIntersectsAABBN(HitDists, box.ChildMins, box.ChildMaxs, r); 
            
            for (int i=0; i < len; i++)
            {
                float distance = HitTest.RayIntersectsAABB(Children[i], r);
                if (distance == 0) //Inside Box = instant try collide
                {
                    HitInfo info = Collide(r, box.Children[i]);
                    if (info != null) return info;
                }
                HitDists[i] = distance;
            }
            int minind = MinIndexExcept0(HitDists);
            if (HitDists[minind] == float.PositiveInfinity) return null; //Minimum Hit = no hit
            if (HitDists[minind] == 0) return null;//No other boxes other than self are hit

            HitInfo minhitinfo = Collide(r, box.Children[minind]); //Try the nearest box first
            if (minhitinfo != null) return minhitinfo;

            for (int i=0;i<len;i++)
            {
                if (i == minind) continue; //Already tested nearest box
                if (HitDists[i] == float.PositiveInfinity || HitDists[i] == 0) continue; //Not hit or Inside (Which we tested already)

                HitInfo info = Collide(r, box.Children[i]);
                if (info != null) return info;
            }

            return null;
        }

        HitInfo GetHit(Ray r, List<Object3D> ToTest) //Array for performance reasons
        {
            int count = ToTest.Count;
            HitInfo mindist = null;
            float nearest = float.MaxValue;
            
            for (int i = 0; i < count; i++)
            {
                HitInfo hit = ToTest[i].Hittest(r);
                if (hit == null || hit.dist > nearest) continue;

                nearest = hit.dist;
                mindist = hit;
            }
            return mindist;
        }

        List<Object3D> ObjectsInbox(AABB box)
        {
            List<Object3D> l = new List<Object3D>();
            var Candidates = box.Parent.ItemsInside;

            for (int i = 0; i < Candidates.Count; i++)
            {
                if (box.Contains(Candidates[i])) l.Add(Candidates[i]);
            }
            return l;
        }

        public void Split(List<Object3D> Objects, int MaxSplit)
        {
            Master = new AABB(min, max);
            Master.SetItems(Objects);
            AllObjects = Objects;
            SplitBox(Master, 0, MaxSplit);
        }

        void SplitBox(AABB Box, int depth, int Maxdepth)
        {
            if (depth == Maxdepth) return;
            if (Box.ItemsInside.Count <= 12) return; //Allow 1-12 Objects per box.

            List<AABB> SubBoxes = Box.Split();
            List<AABB> KeptBoxes = new List<AABB>();
            int ContainedObjects = 0;
            for (int i = 0; i < SubBoxes.Count; i++)
            {
                var Subbox = SubBoxes[i];
                var InsideObj = ObjectsInbox(Subbox);
                if (InsideObj.Count == 0) continue;

                ContainedObjects += InsideObj.Count;
                KeptBoxes.Add(Subbox);
                Subbox.SetItems(InsideObj);
            }
            Box.SetChildren(KeptBoxes);

            for (int i = 0; i < KeptBoxes.Count; i++)
            {
                SplitBox(KeptBoxes[i], depth + 1, Maxdepth); //Still more boxes to split
            }
        }

        public int GetTotalBoxcount()
        {
            return Master.GetTotalBoxcount();
        }

        public int GetTotalItemBoxcount()
        {
            return Master.GetItemBoxcount();
        }

        
    }
}
