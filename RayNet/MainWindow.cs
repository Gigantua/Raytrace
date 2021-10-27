using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;
using OpenTK.Input;

namespace RayNet
{
    public class MainWindow : GameWindow
    {
        System.Drawing.Point? PointDown = null;
        System.Numerics.Vector2 Delta = new System.Numerics.Vector2(0, 0);

        public Camera cam = Camera.Create(new System.Numerics.Vector3(4, 9, 4), new System.Numerics.Vector3(0, 0, 0));

        public MainWindow() : this(1366, 768)
        {
            this.MouseDown += (sender, e) => PointDown = e.Position;
            this.MouseUp += (sender, e) => PointDown = null;
            this.MouseMove += MainWindow_MouseMove;
            this.KeyDown += (sender, e) => keys.Add(e.Key);
            this.KeyUp += (sender, e) => keys.Remove(e.Key);
        }

        private void MainWindow_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            if (PointDown == null) return;
            if (e.Mouse.LeftButton != OpenTK.Input.ButtonState.Pressed) return;
            Delta += new System.Numerics.Vector2(e.X- PointDown.Value.X, e.Y - PointDown.Value.Y);
            PointDown = e.Position;
        }

        public MainWindow(int Width, int Height) : base(Width, Height)
        {
            this.VSync = VSyncMode.Off;
            t = Stopwatch.StartNew();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        Stopwatch t;

        HashSet<Key> keys = new HashSet<Key>();

        void HandleInput(float t)
        {
            t = t * 5;
            if (keys.Contains(Key.W)) cam.MoveForward(t);
            if (keys.Contains(Key.S)) cam.MoveForward(-t);
            if (keys.Contains(Key.A)) cam.MoveRight(-t);
            if (keys.Contains(Key.D)) cam.MoveRight(t);

            if (Delta.X != 0) cam.RotateLeft(Delta.X / 100.0);
            if (Delta.Y != 0) cam.RotateUp(Delta.Y / 100.0);

            Delta = new System.Numerics.Vector2(0, 0);
        }

        protected unsafe override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"RayNet: (Vsync: {VSync}) FPS: {1f / e.Time: 0}";
            //Title = $"{cam.Pos} {cam.Forward}";
            double T = t.Elapsed.TotalSeconds; t.Restart();
            HandleInput((float)T);

            OpenTK.Vector2 Screen = new OpenTK.Vector2(this.Width, this.Height);
            GL.UseProgram(_program);
            GL.VertexAttrib1(0, T);
           
            GL.VertexAttrib2(1, Screen);

            GL.VertexAttrib3(2, ref Unsafe.AsRef(cam.Pos.X));
            GL.VertexAttrib3(3, ref Unsafe.AsRef(cam.Forward.X));
            GL.VertexAttrib3(4, ref Unsafe.AsRef(cam.Up.X));
            GL.VertexAttrib3(5, ref Unsafe.AsRef(cam.Right.X));

            /*
            GL.BindBuffer(BufferTarget.UniformBuffer, _buffer);
            Span<float> values = new Span<float>(GL.MapBuffer(BufferTarget.UniformBuffer, BufferAccess.WriteOnly).ToPointer(), 4 * 3 * 4);
            values[0] = cam.Pos.X; values[1] = cam.Pos.Y; values[2] = cam.Pos.Z;
            values[3] = cam.Forward.X; values[4] = cam.Forward.Y; values[5] = cam.Forward.Z;
            values[6] = cam.Up.X; values[7] = cam.Up.Y; values[8] = cam.Up.Z;
            values[9] = cam.Right.X; values[10] = cam.Right.Y; values[11] = cam.Right.Z;
            GL.UnmapBuffer(BufferTarget.UniformBuffer); //copy values
            */

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            SwapBuffers();

        }

        private int CompileShaders()
        {
            Shader s = new Shader();
            s.AddVertexShader(@"Shaders\vertexShader.vert");
            s.AddFragmentShader(@"Shaders\fragmentShader.frag");
            return s.Compile();
        }
        private int _program;
        private int _buffer;

        protected override void OnLoad(EventArgs e)
        {
            CursorVisible = true;
            _program = CompileShaders();
            _buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _buffer);
            GL.BufferData(BufferTarget.UniformBuffer, 4 * 3 * 4, IntPtr.Zero, BufferUsageHint.StreamDraw); //this can be static if positions do not change
            GL.UnmapBuffer(BufferTarget.UniformBuffer);
           
            int block_index = GL.GetUniformBlockIndex(_program, "shader_data");
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, block_index, _buffer);

            Closed += OnClosed;
        }
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            GL.DeleteProgram(_program);
            base.Exit();
        }

        

    }
}
