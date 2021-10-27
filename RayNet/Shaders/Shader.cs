using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace RayNet
{
    public class Shader
    {
        int vert = -1;
        int frag = -1;

        int compile(ShaderType type, string Source)
        {
            int handle = GL.CreateShader(type);
            GL.ShaderSource(handle, Source);
            GL.CompileShader(handle);
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int completed);
            if (completed != 1) throw new ArgumentException(GL.GetShaderInfoLog(handle));
            return handle;
        }

        public void AddVertexShader(string SourcePath)
        {
            vert = compile(ShaderType.VertexShader, File.ReadAllText(SourcePath));
        }

        public void AddFragmentShader(string SourcePath)
        {
            frag = compile(ShaderType.FragmentShader, File.ReadAllText(SourcePath));
        }

        public int Compile()
        {
            int program = GL.CreateProgram();
            GL.AttachShader(program, vert);
            GL.AttachShader(program, frag);
            GL.LinkProgram(program);

            GL.DetachShader(program, vert);
            GL.DetachShader(program, frag);
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
            return program;
        }

    }
}
