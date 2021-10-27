using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine2.Primitives;

namespace Engine2.Raytrace
{
    
    public class BoundedBox
    {
        public Triangle[] ContainedVertices;
        public BoundedBox[] Boxes = new BoundedBox[0];
        public BoundingBox Box;
        public BoundingSphere BoundingSphere;
        public BoundedBox Parent;
        public OrientedBoundingBox oriented;

        public override string ToString()
        {
            var min = Box.Minimum;
            var max = Box.Maximum;

            return $"{min.X.ToString("0.0000")}|{min.Y.ToString("0.0000")}|{min.Z.ToString("0.0000")} {max.X.ToString("0.0000")}|{max.Y.ToString("0.0000")}|{max.Z.ToString("0.0000")}";
        }

        public BoundedBox(Triangle[] vertices)
        {
            var realvertices = vertices.SelectMany(x => x.Elements).ToArray();
            this.ContainedVertices = vertices;
            this.oriented = new OrientedBoundingBox(realvertices);
            this.Box = BoundingBox.FromPoints(realvertices);
            this.BoundingSphere = BoundingSphere.FromPoints(realvertices);
        }

        public BoundedBox(BoundedBox Parent, BoundingBox Box, Triangle[] vertices)
        {
            var realvertices = vertices.SelectMany(x => x.Elements).ToArray();

            this.Box = Box;
            this.ContainedVertices = vertices.Where(x => x.IsIntersected(Box)).ToArray();
            this.oriented = new OrientedBoundingBox(realvertices);
            this.Parent = Parent;
        }

        BoundingBox CreateBoundingBox(Vector3 start, float dx, float dy, float dz)
        {
            return new BoundingBox(start, new Vector3(start.X + dx, start.Y + dy, start.Z + dz));
        }

        Vector3 VXYZ(Vector3 start, float dX, float dY, float dZ)
        {
            return new Vector3(start.X + dX, start.Y + dY, start.Z + dZ);
        }

        /// <summary>
        /// Returns maximum amount of vertices in a splitted
        /// </summary>
        public void Split(int level)
        {
            if (ContainedVertices.Length == 0) return;
            if (level == 4) return;

            float dx = Box.Maximum.X - Box.Minimum.X;
            float dy = Box.Maximum.Y - Box.Minimum.Y;
            float dz = Box.Maximum.Z - Box.Minimum.Z;

            if (dx < 1e-7) return;
            if (dy < 1e-7) return;
            if (dz < 1e-7) return;

            float dxh = dx / 2;
            float dyh = dy / 2;
            float dzh = dz / 2;
            Vector3 min = Box.Minimum;

            Boxes = new BoundedBox[8];
            Boxes[0] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, 0, 0, 0), dxh, dyh, dzh), ContainedVertices);
            Boxes[1] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, dxh, 0, 0), dxh, dyh, dzh), ContainedVertices);
            Boxes[2] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, 0, 0, dzh), dxh, dyh, dzh), ContainedVertices);
            Boxes[3] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, dxh, 0, dzh), dxh, dyh, dzh), ContainedVertices);

            Boxes[4] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, 0, dyh, 0), dxh, dyh, dzh), ContainedVertices);
            Boxes[5] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, dxh, dyh, 0), dxh, dyh, dzh), ContainedVertices);
            Boxes[6] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, 0, dyh, dzh), dxh, dyh, dzh), ContainedVertices);
            Boxes[7] = new BoundedBox(this, CreateBoundingBox(VXYZ(min, dxh, dyh, dzh), dxh, dyh, dzh), ContainedVertices);


            for (int i = 0; i < Boxes.Length; i++)
            {
                if (ContainedVertices.Length > 48 && Boxes[i].ContainedVertices.Length != 0) Boxes[i].Split(level+1); //Split as long as there are more than 12 vertices in parent
                if (Boxes[i].ContainedVertices.Length == 0) Boxes[i] = null;
            }
            Boxes = Boxes.Where(x => x != null).ToArray();
        }

        void GetIntersectionsInternal(Ray ray,List<BoundedBox> items)
        {
            if (ray.Intersects(ref this.Box) == false) return;

            if (this.Boxes.Length == 0) //No more children to be hit = WE ARE THE WINNER
            {
                items.Add(this);
            }
            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].GetIntersectionsInternal(ray, items);
            }
        }


        public List<BoundedBox> GetIntersections(Ray ray)
        {
            if (ray.Intersects(ref this.Box) == false) return null; //we are not intersected at all

            List<BoundedBox> toreturn = new List<BoundedBox>();
            GetIntersectionsInternal(ray, toreturn);
            return toreturn;
        }

        public bool IsIntersected(Ray ray)
        {
            if (ray.Intersects(ref this.Box) == false) return false;
            return GetIsIntersectionedInternal(ray);
        }

        bool GetIsIntersectionedInternal(Ray ray)
        {
            if (ray.Intersects(ref this.Box) == false) return false;
            if (this.Boxes.Length == 0) return true; //No more children to be hit = WE ARE THE WINNER

            for (int i = 0; i < Boxes.Length; i++)
            {
                if (Boxes[i].GetIsIntersectionedInternal(ray))
                {
                    return true;
                }
            }
            return false; //no children hit
        }

    }

    public class RecursiveBox
    {
        /// <summary>
        /// Does recursive bounding until the smallest box contains 4 elements
        /// </summary>

        public BoundedBox box;

        public RecursiveBox(GameObject monkey)
        {
            Face[] faces = monkey.Faces;
            
            List<Triangle> ts = new List<Triangle>();
            for (int i = 0; i < faces.Length; i++)
            {
                Face face = faces[i];
                var vertexA = monkey.Vertices[face.A];
                var vertexB = monkey.Vertices[face.B];
                var vertexC = monkey.Vertices[face.C];
                ts.Add(new Triangle(vertexA,vertexB,vertexC));
            }
            box = new BoundedBox(ts.ToArray());
        }

        public void Calculate()
        {
            box.Split(0);
        }

        public Triangle Intersect(Ray ray)
        {
            if (box.BoundingSphere.Intersects(ref ray) == false) return null;
            List<BoundedBox> boxes = box.GetIntersections(ray);
            if (boxes == null) return null;

            float mindist = float.PositiveInfinity;
            Triangle closest = null;

            for (int i=0;i<boxes.Count;i++)
            {
                BoundedBox box = boxes[i];
                for (int r = 0; r<box.ContainedVertices.Length;r++)
                {
                    var (isinter, dist) = box.ContainedVertices[r].RayIntersection(ray);
                    if (!isinter) continue;

                    if (dist<mindist)
                    {
                        mindist = dist;
                        closest = box.ContainedVertices[r];
                    }
                }
            }

            return closest;
        }

        public bool IsIntersected(Ray ray)
        {
            List<BoundedBox> boxes = box.GetIntersections(ray);
            if (boxes == null) return false;

            for (int i = 0; i < boxes.Count; i++)
            {
                BoundedBox box = boxes[i];
                for (int r = 0; r < box.ContainedVertices.Length; r++)
                {
                    var (isinter, dist) = box.ContainedVertices[r].RayIntersection(ray);
                    if (isinter) return true;
                }
            }

            return false;
        }


    }
}
