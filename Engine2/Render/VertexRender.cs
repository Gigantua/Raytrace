using Engine2.Primitives;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Render
{
    class VertexRender
    {
        Bitmap_float Target;
        GameObject[] Objects;
        int Width;
        int Height;
        ZBuffer ZBuf;
        Matrix Projectionview;
        Camera cam;
        BoundingFrustum frustrum;
        GameEngine Engine;
        Plane floor = new Plane(new Vector3(0, -1, 0), new Vector3(0, 1, 0));

        public VertexRender(Bitmap_float Target,ZBuffer zbuf, Camera cam, GameObject[] Objects, GameEngine Engine)
        {
            this.Width = Target.Width;
            this.Height = Target.Height;
            this.ZBuf = zbuf;
            this.Target = Target;
            this.Objects = Objects;
            this.cam = cam;
            this.Engine = Engine;
        }

        public void Draw()
        {
            var ViewMatrix = Matrix.LookAtLH(cam.Position, cam.Target, Vector3.UnitY);
            var ProjectMatrix = Matrix.PerspectiveFovLH(1f, (float)Width / Height, 0.01f, 1.0f);
            Projectionview = ViewMatrix * ProjectMatrix; //Is a matrix for unmovable objects
            frustrum = BoundingFrustum.FromCamera(cam.Position, cam.Direction, cam.Up, 1, 0.1f, 100000.0f, (float)Width / Height);
            
            for (int i=0;i<Objects.Length;i++)
            {
                Draw(Objects[i]);
            }
        }

        public void Draw(GameObject obj)
        {
            Matrix TransformMatrix = obj.TransformMatrix;
            var worldviewprojection = TransformMatrix * Projectionview;

            Face[] faces = obj.Faces;
            Parallel.For(0, faces.Length, (faceIndex) =>
            {
                Face face = faces[faceIndex];
                var vertexA = obj.Vertices[face.A];
                var vertexB = obj.Vertices[face.B];
                var vertexC = obj.Vertices[face.C];

                var pixelA = Project3D(vertexA, worldviewprojection, TransformMatrix);
                var pixelB = Project3D(vertexB, worldviewprojection, TransformMatrix);
                var pixelC = Project3D(vertexC, worldviewprojection, TransformMatrix);

                if 
                (
                    frustrum.Contains(pixelA.WorldCoords) == ContainmentType.Contains ||
                    frustrum.Contains(pixelB.WorldCoords) == ContainmentType.Contains ||
                    frustrum.Contains(pixelC.WorldCoords) == ContainmentType.Contains
                )
                {
                    float color = 1f;
                    DrawTriangle(pixelA, pixelB, pixelC, Color4.FromElements(color, color, color, 1));
                }
            });

            var boxes = obj.Bounds.box.Boxes.SelectMany(x => x.Boxes).SelectMany(x => x.Boxes).Select(x => x.Box);
            DrawBox(obj.Bounds.box.Box, worldviewprojection);
            foreach (var item in boxes)
            {
                //DrawBox(item, worldviewprojection);
            }
        }

        static Color4 pen = Colorconvert.FromColor(System.Drawing.Color.FromArgb(150, System.Drawing.Color.Red));

        public Vertex Project3D(Vertex vertex, Matrix transMat, Matrix world)
        {
            var point2d = Vector3.TransformCoordinate(vertex.Coordinates, transMat);
            // transforming the coordinates & the normal to the vertex in the 3D world
            var point3dWorld = Vector3.TransformCoordinate(vertex.Coordinates, world);
            var normal3dWorld = Vector3.TransformCoordinate(vertex.Normal, world);


            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point2d.X * Width + Width / 2.0f;
            var y = -point2d.Y * Height + Height / 2.0f;

            return new Vertex
            {
                Coordinates = new Vector3(x, y, point2d.Z),
                Normal = normal3dWorld,
                WorldCoords = point3dWorld
            };
        }

        void DrawLine(int x0, int y0, int x1, int y1)
        {
            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;
            Color4[] backbuf = Target.Pixel;
            int width = Target.Width;
            int len = backbuf.Length - 1;

            if (x1 < 0) return;
            if (y1 < 0) return;

            if (x0 >= Width) return;
            if (y0 >= Height) return;

            while (true)
            {
                int index = width * y0 + x0;
                int clamped = (index < 0) ? 0 : (index > len) ? len : index;
                backbuf[clamped] += pen;
                if ((x0 == x1) && (y0 == y1)) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        Vector2 Coord(Vector3 v)
        {
            var x = v.X * Width + Width / 2.0f;
            var y = -v.Y * Height + Height / 2.0f;

            return new Vector2(x, y);
        }

        void DrawQuad(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Matrix worldviewprojection)
        {
            var pixelA = Coord(Vector3.TransformCoordinate(A, worldviewprojection));
            var pixelB = Coord(Vector3.TransformCoordinate(B, worldviewprojection));
            var pixelC = Coord(Vector3.TransformCoordinate(C, worldviewprojection));
            var pixelD = Coord(Vector3.TransformCoordinate(D, worldviewprojection));

            DrawLine((int)pixelA.X, (int)pixelA.Y, (int)pixelB.X, (int)pixelB.Y);
            DrawLine((int)pixelB.X, (int)pixelB.Y, (int)pixelC.X, (int)pixelC.Y);
            DrawLine((int)pixelC.X, (int)pixelC.Y, (int)pixelD.X, (int)pixelD.Y);
            DrawLine((int)pixelD.X, (int)pixelD.Y, (int)pixelA.X, (int)pixelA.Y);
        }
        void DrawBox(BoundingBox box, Matrix A)
        {
            float dx = box.Maximum.X - box.Minimum.X;
            float dy = box.Maximum.Y - box.Minimum.Y;
            float dz = box.Maximum.Z - box.Minimum.Z;

            Vector3 min = box.Minimum;
            Vector3 max = box.Maximum;

            DrawQuad(VXYZ(min, 0, 0, 0), VXYZ(min, dx, 0, 0), VXYZ(min, dx, dy, 0), VXYZ(min, 0, dy, 0), A); //front
            DrawQuad(VXYZ(min, 0, 0, dz), VXYZ(min, dx, 0, dz), VXYZ(min, dx, dy, dz), VXYZ(min, 0, dy, dz), A); //back

            DrawQuad(VXYZ(min, 0, 0, 0), VXYZ(min, dx, 0, 0), VXYZ(min, dx, 0, dz), VXYZ(min, 0, 0, dz), A); //bottom
            DrawQuad(VXYZ(min, 0, dy, 0), VXYZ(min, dx, dy, 0), VXYZ(min, dx, dy, dz), VXYZ(min, 0, dy, dz), A); //top
        }
        Vector3 VXYZ(Vector3 start, float dX, float dY, float dZ)
        {
            return new Vector3(start.X + dX, start.Y + dY, start.Z + dZ);
        }

        // Clamping values to keep them between 0 and 1
        float Clamp(float value, float min = 0, float max = 1)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        int ClamptoSize(int value, int min = 0, int max = 1)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        Vector3 ClampVertex(Vertex v)
        {
            return new Vector3(Clamp(v.Coordinates.X, 0, Width - 1), Clamp(v.Coordinates.Y, 0, Height - 1), v.Coordinates.Z);
        }

        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }
        void ProcessScanLine(ScanLineData data, Vertex va, Vertex vb, Vertex vc, Vertex vd, Color4 color)
        {
            if (data.currentY < 0 || data.currentY >= Height) return;

            Vector3 pa = va.Coordinates;
            Vector3 pb = vb.Coordinates;
            Vector3 pc = vc.Coordinates;
            Vector3 pd = vd.Coordinates;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pa.Y != pb.Y ? (data.currentY - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (data.currentY - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            if (sx >= Width && ex >= Width) return; 
            if (sx < 0 && ex < 0) return; //return if start and end is not visible

            sx = ClamptoSize(sx, 0, Width);
            ex = ClamptoSize(ex, 0, Width);
            if (sx == ex) return; //return if clamped start is outside

            // starting Z & ending Z
            float z1 = Interpolate(pa.Z, pb.Z, gradient1);
            float z2 = Interpolate(pc.Z, pd.Z, gradient2);

            var snl = Interpolate(data.ndotla, data.ndotlb, gradient1);
            var enl = Interpolate(data.ndotlc, data.ndotld, gradient2);

            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                var ndotl = Interpolate(snl, enl, gradient);
                // changing the color value using the cosine of the angle
                // between the light vector and the normal vector

                ZBuf.DrawPoint(x, data.currentY, z, color * ndotl, Target);
            }
        }

        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        float ComputeNDotL(Vector3 vertex, Vector3 normal, Vector3 lightPosition)
        {
            var lightDirection = lightPosition - vertex;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector3.Dot(normal, lightDirection));
        }

        public struct ScanLineData
        {
            public int currentY;
            public float ndotla;
            public float ndotlb;
            public float ndotlc;
            public float ndotld;
        }

        public void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Color4 color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            if (v2.Coordinates.Y > v3.Coordinates.Y)
            {
                var temp = v2;
                v2 = v3;
                v3 = temp;
            }

            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            Vector3 p1 = v1.Coordinates;
            Vector3 p2 = v2.Coordinates;
            Vector3 p3 = v3.Coordinates;


            // computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color
            float nl1 = ComputeNDotL(v1.WorldCoords, v1.Normal, Engine.LightPos);
            float nl2 = ComputeNDotL(v2.WorldCoords, v2.Normal, Engine.LightPos);
            float nl3 = ComputeNDotL(v3.WorldCoords, v3.Normal, Engine.LightPos);

            var data = new ScanLineData { };

            // computing lines' directions
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            int starty = ClamptoSize((int)p1.Y, 0, Height - 1);
            int endy = ClamptoSize((int)p3.Y, 0, Height - 1);

            if (dP1P2 > dP1P3)
            {
                for (var y = starty; y <= endy; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl2;
                        ProcessScanLine(data, v1, v3, v1, v2, color);
                    }
                    else
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl2;
                        data.ndotld = nl3;
                        ProcessScanLine(data, v1, v3, v2, v3, color);
                    }
                }
            }
            else
            {
                for (var y = starty; y <= endy; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl2;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLine(data, v1, v2, v1, v3, color);
                    }
                    else
                    {
                        data.ndotla = nl2;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLine(data, v2, v3, v1, v3, color);
                    }
                }
            }
        }

    }
}
