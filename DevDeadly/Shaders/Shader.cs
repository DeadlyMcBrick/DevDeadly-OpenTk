using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DevDeadly.Shaders
{
    public class Shader
    {
        public int Handle;
        private bool disposedValue = false;
        public int programID;
        Shader shader;
        private Stopwatch timer = Stopwatch.StartNew();

        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine("Program link error?" + infoLog);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }


        private void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine("Shader compile error?: " + infoLog);
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void Dispose()
        {
            if (disposedValue)
                return;

            GL.DeleteProgram(Handle);
            disposedValue = true;
            GC.SuppressFinalize(this);
        }

        public int GetAttribLocation(string name)
        {
            return GL.GetAttribLocation(Handle, name);
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, value);
        }

        public void VertLocation(string name, int value)
        {
            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            if (vertexColorLocation == -1)
            {

            }
            else
            {
                double timeValue = timer.Elapsed.TotalSeconds;
                float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
                GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
            }
        }

        ~Shader()
        {
            if (!disposedValue)
            {
                Console.WriteLine("GPU Resource Leak, Yoo man! Did I forget to call Dispose?");
            }
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            GL.UseProgram(Handle);
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, true, ref matrix);
        }

        public void SetBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value ? 1 : 0);
        }

        public void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location != -1)
            {
                GL.Uniform3(location, vector);
            }
            else
            {
                Console.WriteLine($"Uniform '{name}' is not being found.");
            }
        }
    }
}