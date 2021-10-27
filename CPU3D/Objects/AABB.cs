using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPU3D.RayTrace;

namespace CPU3D.Objects
{
    public class AABB : Box
    {
        public int GetTotalBoxcount()
        {
            int r = 0;
            BoxCount(ref r, true);
            return 1 + r;
        }

        public int GetItemBoxcount()
        {
            int r = 0;
            BoxCount(ref r, false);
            return r;
        }


        protected void BoxCount(ref int n, bool AllBoxes)
        {
            if (AllBoxes) n += ChildCount;
            else if (ChildCount == 0) n += 1;
            
            for(int i=0;i<ChildCount;i++)
            {
                Children[i].BoxCount(ref n, AllBoxes);
            }
        }

        public AABB Parent;
        public List<AABB> Children { get; private set; }
        public List<Object3D> ItemsInside { get; private set; }
        public int ChildCount { get; private set; }

        public AABB(Vector3 min, Vector3 max) : base(min, max)
        {
            Vector3 minr = Vector3.Min(min, max);
            Vector3 maxr = Vector3.Max(min, max);

            Vector3 Center = (maxr + minr) / 2;
        }

        public override string ToString()
        {
            return $"{min} - {max}";
        }
        
        public override HitInfo Hittest(Ray r)
        {
            float dist = HitTest.RayIntersectsAABB(this, r);
            if (dist == float.MaxValue) return null;

            return new HitInfo(r);
        }

        /// <summary>
        /// Splits this box into 8 children
        /// </summary>
        /// <returns></returns>
        public List<AABB> Split()
        {
            Vector3 Center = (min + max) / 2;
            Vector3 dX = Vector3.UnitX * (max.X - min.X) / 2;
            Vector3 dY = Vector3.UnitY * (max.Y - min.Y) / 2;
            Vector3 dZ = Vector3.UnitZ * (max.Z - min.Z) / 2;
            AABB a1 = new AABB(min, Center) { Parent = this };
            AABB a2 = new AABB(min + dX, Center + dX) { Parent = this };
            AABB a3 = new AABB(min + dY, Center + dY) { Parent = this };
            AABB a4 = new AABB(min + dX + dY, Center + dX + dY) { Parent = this };

            Vector3 min2 = min + dZ;
            Vector3 Center2 = Center + dZ;

            AABB b1 = new AABB(min2, Center2) { Parent = this };
            AABB b2 = new AABB(min2 + dX, Center2 + dX) { Parent = this };
            AABB b3 = new AABB(min2 + dY, Center2 + dY) { Parent = this };
            AABB b4 = new AABB(min2 + dX + dY, Center2 + dX + dY) { Parent = this };

            return new List<AABB>()
            {
                a1,a2,a3,a4,b1,b2,b3,b4
            };
        }

        int NextMultiple(int x, int M)
        {
            if (x % M == 0) return x;
            return x + (M - x % M); //6 -> 8, 8->8, 12->16 for any x, M=8
        }

        public void SetChildren(List<AABB> Childboxes)
        {
            Children = Childboxes;
            ChildCount = Childboxes.Count;
        }

        public void SetItems(List<Object3D> Items)
        {
            ItemsInside = Items;
        }

        public bool Contains(Object3D obj)
        {
            if (obj is Box s)
            {
                return HitTest.BoxIntersectsBox(this, s);
            }
            else
            {
                return HitTest.BoxIntersectsBox(this, obj.HitBox);
            }

        }

    }
}
